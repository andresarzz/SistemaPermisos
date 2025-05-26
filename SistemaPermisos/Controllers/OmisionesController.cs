using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Data;
using SistemaPermisos.Models;
using SistemaPermisos.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using SistemaPermisos.Services;

namespace SistemaPermisos.Controllers
{
    [Authorize]
    public class OmisionesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public OmisionesController(ApplicationDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        // GET: Omisiones
        public async Task<IActionResult> Index()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var rol = HttpContext.Session.GetString("UsuarioRol");

            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _context.OmisionesMarca.Include(o => o.Usuario).AsQueryable();

            // Si no es administrador, solo mostrar sus propias omisiones
            if (rol != "Admin" && rol != "Director")
            {
                query = query.Where(o => o.UsuarioId == usuarioId);
            }

            var omisiones = await query.OrderByDescending(o => o.FechaRegistro).ToListAsync();
            return View(omisiones);
        }

        // GET: Omisiones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var omision = await _context.OmisionesMarca
                .Include(o => o.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (omision == null)
            {
                return NotFound();
            }

            // Verificar si el usuario tiene permiso para ver esta omisión
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var rol = HttpContext.Session.GetString("UsuarioRol");

            if (rol != "Admin" && rol != "Director" && omision.UsuarioId != usuarioId)
            {
                return Forbid();
            }

            return View(omision);
        }

        // GET: Omisiones/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Omisiones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OmisionMarca omision)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            omision.UsuarioId = usuarioId.Value;
            omision.FechaRegistro = DateTime.Now;
            omision.Estado = "Pendiente";

            if (ModelState.IsValid)
            {
                _context.Add(omision);
                await _context.SaveChangesAsync();

                // Registrar en auditoría
                await _auditService.LogActivityAsync(
                    usuarioId.Value,
                    "Crear",
                    "OmisionMarca",
                    omision.Id,
                    null,
                    $"Nueva omisión de marca: {omision.TipoOmision} - {omision.FechaOmision:dd/MM/yyyy}"
                );

                return RedirectToAction(nameof(Index));
            }
            return View(omision);
        }

        // GET: Omisiones/Resolve/5
        [Authorize(Roles = "Admin,Director")]
        public async Task<IActionResult> Resolve(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var omision = await _context.OmisionesMarca
                .Include(o => o.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (omision == null)
            {
                return NotFound();
            }

            if (omision.Estado != "Pendiente")
            {
                TempData["ErrorMessage"] = "Esta omisión ya ha sido resuelta.";
                return RedirectToAction(nameof(Details), new { id = omision.Id });
            }

            ViewBag.Omision = omision;
            return View(new ResolucionOmisionViewModel { OmisionId = omision.Id });
        }

        // POST: Omisiones/Resolve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Director")]
        public async Task<IActionResult> Resolve(ResolucionOmisionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var omision = await _context.OmisionesMarca.FindAsync(model.OmisionId);

                if (omision == null)
                {
                    return NotFound();
                }

                if (omision.Estado != "Pendiente")
                {
                    TempData["ErrorMessage"] = "Esta omisión ya ha sido resuelta.";
                    return RedirectToAction(nameof(Details), new { id = omision.Id });
                }

                // Actualizar el estado y la resolución
                omision.Estado = "Resuelto";
                omision.Resolucion = model.Resolucion;
                omision.ObservacionesResolucion = model.ObservacionesResolucion;

                _context.Update(omision);
                await _context.SaveChangesAsync();

                // Registrar en auditoría
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                await _auditService.LogActivityAsync(
                    usuarioId,
                    "Resolver",
                    "OmisionMarca",
                    omision.Id,
                    $"Estado anterior: Pendiente",
                    $"Resolución: {model.Resolucion}"
                );

                TempData["SuccessMessage"] = "La omisión ha sido resuelta correctamente.";
                return RedirectToAction(nameof(Details), new { id = omision.Id });
            }

            // Si llegamos aquí, algo falló, volvemos a mostrar el formulario
            var omisionModel = await _context.OmisionesMarca
                .Include(o => o.Usuario)
                .FirstOrDefaultAsync(m => m.Id == model.OmisionId);

            ViewBag.Omision = omisionModel;
            return View(model);
        }
    }
}
