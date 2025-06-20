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
using SistemaPermisos.Services;
using Microsoft.AspNetCore.Hosting;

namespace SistemaPermisos.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IAuditService _auditService;

        public ReportesController(
            ApplicationDbContext context,
            IWebHostEnvironment hostEnvironment,
            IAuditService auditService)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _auditService = auditService;
        }

        // GET: Reportes
        public async Task<IActionResult> Index()
        {
            // Verificar si el usuario está autenticado
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var rol = HttpContext.Session.GetString("UsuarioRol");

            // Si es administrador, mostrar todos los reportes
            if (rol == "Admin")
            {
                var reportes = await _context.ReportesDanos
                    .Include(r => r.Usuario)
                    .OrderByDescending(r => r.FechaReporte)
                    .ToListAsync();
                return View(reportes);
            }

            // Si es docente, mostrar solo sus reportes
            var misReportes = await _context.ReportesDanos
                .Include(r => r.Usuario)
                .Where(r => r.UsuarioId == usuarioId)
                .OrderByDescending(r => r.FechaReporte)
                .ToListAsync();

            return View(misReportes);
        }

        // GET: Reportes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reporte = await _context.ReportesDanos
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reporte == null)
            {
                return NotFound();
            }

            // Verificar que el usuario tenga acceso a este reporte
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var rol = HttpContext.Session.GetString("UsuarioRol");

            if (usuarioId == null || (reporte.UsuarioId != usuarioId && rol != "Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            return View(reporte);
        }

        // GET: Reportes/Create
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

        // POST: Reportes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReporteViewModel model)
        {
            try
            {
                // Verificar si el usuario está autenticado
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState)
                    {
                        Console.WriteLine($"Error en {error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                    return View(model);
                }

                // Procesar la imagen del reporte
                string? rutaImagen = null;
                if (model.Imagen != null && model.Imagen.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "reportes");

                    // Asegurar que el directorio existe
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Imagen.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Imagen.CopyToAsync(fileStream);
                    }

                    rutaImagen = "/uploads/reportes/" + uniqueFileName;
                }

                var reporte = new ReporteDano
                {
                    UsuarioId = usuarioId.Value,
                    Equipo = model.Equipo ?? string.Empty,
                    Ubicacion = model.Ubicacion ?? string.Empty,
                    Descripcion = model.Descripcion ?? string.Empty,
                    RutaImagen = rutaImagen,
                    Estado = "Pendiente",
                    FechaReporte = DateTime.Now
                };

                _context.Add(reporte);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Reporte guardado con ID: {reporte.Id}");

                // Registrar en auditoría
                try
                {
                    await _auditService.LogActivityAsync(
                        usuarioId,
                        "Crear",
                        "ReporteDano",
                        reporte.Id,
                        null,
                        $"Nuevo reporte: {reporte.Equipo} - {reporte.Ubicacion}"
                    );
                }
                catch (Exception auditEx)
                {
                    Console.WriteLine($"Error en auditoría: {auditEx.Message}");
                    // No fallar por error de auditoría
                }

                TempData["Success"] = "Reporte enviado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear reporte: {ex.Message}");
                ModelState.AddModelError("", "Ocurrió un error al procesar su solicitud. Por favor, inténtelo de nuevo.");
                return View(model);
            }
        }

        // GET: Reportes/MarkAsResolved/5 (Solo para administradores)
        public async Task<IActionResult> MarkAsResolved(int? id)
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

            var reporte = await _context.ReportesDanos.FindAsync(id);
            if (reporte == null)
            {
                return NotFound();
            }

            reporte.Estado = "Resuelto";
            reporte.FechaResolucion = DateTime.Now;
            reporte.ResueltoPor = HttpContext.Session.GetString("UsuarioNombre");

            await _context.SaveChangesAsync();

            // Registrar en auditoría
            await _auditService.LogActivityAsync(
                HttpContext.Session.GetInt32("UsuarioId"),
                "Actualizar",
                "ReporteDano",
                reporte.Id,
                "Estado anterior: Pendiente",
                "Nuevo estado: Resuelto"
            );

            return RedirectToAction(nameof(Index));
        }
    }
}
