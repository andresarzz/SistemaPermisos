using System;
using System.IO;
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
    public class PermisosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PermisosController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Permisos
        public async Task<IActionResult> Index()
        {
            // Verificar si el usuario está autenticado
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var rol = HttpContext.Session.GetString("UsuarioRol");

            // Si es administrador, mostrar todos los permisos
            if (rol == "Admin")
            {
                var permisos = await _context.Permisos
                    .Include(p => p.Usuario)
                    .OrderByDescending(p => p.FechaSolicitud)
                    .ToListAsync();
                return View(permisos);
            }

            // Si es docente, mostrar solo sus permisos
            var misPermisos = await _context.Permisos
                .Include(p => p.Usuario)
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.FechaSolicitud)
                .ToListAsync();

            return View(misPermisos);
        }

        // GET: Permisos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permiso = await _context.Permisos
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (permiso == null)
            {
                return NotFound();
            }

            // Verificar que el usuario tenga acceso a este permiso
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var rol = HttpContext.Session.GetString("UsuarioRol");

            if (usuarioId == null || (permiso.UsuarioId != usuarioId && rol != "Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            return View(permiso);
        }

        // GET: Permisos/Create
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

        // POST: Permisos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PermisoViewModel model)
        {
            // Verificar si el usuario está autenticado
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                // Procesar la imagen del comprobante
                string rutaComprobante = null;
                if (model.Comprobante != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "comprobantes");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Comprobante.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Asegurar que el directorio existe
                    Directory.CreateDirectory(uploadsFolder);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Comprobante.CopyToAsync(fileStream);
                    }

                    rutaComprobante = "/uploads/comprobantes/" + uniqueFileName;
                }

                var permiso = new Permiso
                {
                    UsuarioId = usuarioId.Value,
                    FechaSalida = model.FechaSalida,
                    FechaRegreso = model.FechaRegreso,
                    Motivo = model.Motivo,
                    RutaComprobante = rutaComprobante,
                    RutaJustificacion = "", // Asignar cadena vacía en lugar de null
                    Estado = "Pendiente",
                    FechaSolicitud = DateTime.Now
                };

                _context.Add(permiso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Permisos/Justify/5
        public async Task<IActionResult> Justify(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permiso = await _context.Permisos.FindAsync(id);
            if (permiso == null)
            {
                return NotFound();
            }

            // Verificar que el usuario tenga acceso a este permiso
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null || permiso.UsuarioId != usuarioId)
            {
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new JustificarPermisoViewModel
            {
                PermisoId = permiso.Id
            };

            return View(viewModel);
        }

        // POST: Permisos/Justify/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Justify(JustificarPermisoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var permiso = await _context.Permisos.FindAsync(model.PermisoId);
                if (permiso == null)
                {
                    return NotFound();
                }

                // Verificar que el usuario tenga acceso a este permiso
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null || permiso.UsuarioId != usuarioId)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Procesar la imagen de justificación
                if (model.Justificacion != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "justificaciones");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Justificacion.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Asegurar que el directorio existe
                    Directory.CreateDirectory(uploadsFolder);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Justificacion.CopyToAsync(fileStream);
                    }

                    permiso.RutaJustificacion = "/uploads/justificaciones/" + uniqueFileName;
                    permiso.Justificado = true;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(model);
        }

        // GET: Permisos/Approve/5 (Solo para administradores)
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

            var permiso = await _context.Permisos.FindAsync(id);
            if (permiso == null)
            {
                return NotFound();
            }

            permiso.Estado = "Aprobado";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Permisos/Reject/5 (Solo para administradores)
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

            var permiso = await _context.Permisos.FindAsync(id);
            if (permiso == null)
            {
                return NotFound();
            }

            permiso.Estado = "Rechazado";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

