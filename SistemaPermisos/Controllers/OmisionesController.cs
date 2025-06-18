using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Data;
using SistemaPermisos.Models;
using SistemaPermisos.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SistemaPermisos.Services;

namespace SistemaPermisos.Controllers
{
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

                await _auditService.LogActivityAsync(
                    usuarioId.Value,
                    "Crear",
                    "OmisionMarca",
                    omision.Id,
                    null,
                    $"Nueva omisión de marca: {omision.TipoOmision} - {omision.FechaOmision:dd/MM/yyyy}"
                );

                TempData["SuccessMessage"] = "Omisión registrada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(omision);
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

            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var rol = HttpContext.Session.GetString("UsuarioRol");

            if (rol != "Admin" && rol != "Director" && omision.UsuarioId != usuarioId)
            {
                return Forbid();
            }

            return View(omision);
        }
    }
}
