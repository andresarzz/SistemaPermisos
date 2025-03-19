using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Data;
using SistemaPermisos.Models;
using SistemaPermisos.ViewModels;

namespace SistemaPermisos.Controllers
{
    public class OmisionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OmisionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Omisiones
        public async Task<IActionResult> Index()
        {
            // Verificar si el usuario está autenticado
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var rol = HttpContext.Session.GetString("UsuarioRol");

            // Si es administrador, mostrar todas las omisiones
            if (rol == "Admin")
            {
                var omisiones = await _context.OmisionesMarca
                    .Include(o => o.Usuario)
                    .OrderByDescending(o => o.FechaRegistro)
                    .ToListAsync();
                return View(omisiones);
            }

            // Si es docente, mostrar solo sus omisiones
            var misOmisiones = await _context.OmisionesMarca
                .Include(o => o.Usuario)
                .Where(o => o.UsuarioId == usuarioId)
                .OrderByDescending(o => o.FechaRegistro)
                .ToListAsync();

            return View(misOmisiones);
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

            // Verificar que el usuario tenga acceso a esta omisión
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var rol = HttpContext.Session.GetString("UsuarioRol");

            if (usuarioId == null || (omision.UsuarioId != usuarioId && rol != "Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            return View(omision);
        }

        // GET: Omisiones/Create
        public IActionResult Create()
        {
            // Verificar si el usuario está autenticado
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // POST: Omisiones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OmisionViewModel model)
        {
            // Verificar si el usuario está autenticado
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var omision = new OmisionMarca
                {
                    UsuarioId = usuarioId.Value,
                    FechaOmision = model.FechaOmision,
                    TipoOmision = model.TipoOmision,
                    Motivo = model.Motivo,
                    Estado = "Pendiente",
                    FechaRegistro = DateTime.Now
                };

                _context.Add(omision);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Omisiones/Approve/5 (Solo para administradores)
        public async Task<IActionResult> Approve(int? id)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var omision = await _context.OmisionesMarca.FindAsync(id);
            if (omision == null)
            {
                return NotFound();
            }

            omision.Estado = "Aprobado";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Omisiones/Reject/5 (Solo para administradores)
        public async Task<IActionResult> Reject(int? id)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var omision = await _context.OmisionesMarca.FindAsync(id);
            if (omision == null)
            {
                return NotFound();
            }

            omision.Estado = "Rechazado";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

