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
            var viewModel = new OmisionViewModel
            {
                FechaOmision = DateTime.Today
            };
            return View(viewModel);
        }

        // POST: Omisiones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OmisionViewModel viewModel)
        {
            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

                if (usuarioId == null)
                {
                    return Json(new { error = "Sesión expirada. Por favor, inicie sesión nuevamente." });
                }

                if (ModelState.IsValid)
                {
                    // Mapear ViewModel a Model
                    var omision = new OmisionMarca
                    {
                        UsuarioId = usuarioId.Value,
                        FechaOmision = viewModel.FechaOmision,
                        Cedula = viewModel.Cedula ?? string.Empty,
                        Puesto = viewModel.Puesto ?? string.Empty,
                        Instancia = viewModel.Instancia ?? string.Empty,
                        CategoriaPersonal = viewModel.CategoriaPersonal ?? string.Empty,
                        Titulo = viewModel.Titulo ?? string.Empty,
                        TipoOmision = viewModel.TipoOmision ?? string.Empty,
                        Motivo = viewModel.Motivo ?? string.Empty,
                        Justificacion = viewModel.Motivo ?? string.Empty, // Usar el mismo valor
                        FechaRegistro = DateTime.Now,
                        FechaSolicitud = DateTime.Now,
                        Estado = "Pendiente"
                    };

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

                    return Json(new { success = true, message = "Justificación de omisión registrada correctamente." });
                }
                else
                {
                    // Recopilar errores de validación
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .Select(x => new { Field = x.Key, Errors = x.Value?.Errors.Select(e => e.ErrorMessage) })
                        .ToList();

                    return Json(new
                    {
                        error = "Por favor, corrija los errores en el formulario.",
                        validationErrors = errors
                    });
                }
            }
            catch (Exception ex)
            {
                // Log del error para debugging
                Console.WriteLine($"Error en Create Omision: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                return Json(new
                {
                    error = "Ocurrió un error al procesar su solicitud.",
                    detail = ex.Message
                });
            }
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

        // GET: Omisiones/Resolve/5
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

            // Solo admin puede resolver
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin" && rol != "Director")
            {
                return Forbid();
            }

            var viewModel = new ResolucionOmisionViewModel
            {
                OmisionId = omision.Id,
                TipoOmision = omision.TipoOmision ?? string.Empty,
                FechaOmision = omision.FechaOmision,
                Motivo = omision.Motivo ?? string.Empty,
                NombreSolicitante = omision.Usuario?.Nombre ?? "No especificado",
                Cedula = omision.Cedula,
                Puesto = omision.Puesto,
                Instancia = omision.Instancia,
                CategoriaPersonal = omision.CategoriaPersonal,
                Titulo = omision.Titulo
            };

            ViewBag.Omision = omision; // Para compatibilidad con la vista existente
            return View(viewModel);
        }

        // POST: Omisiones/Resolve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resolve(ResolucionOmisionViewModel viewModel)
        {
            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                var rol = HttpContext.Session.GetString("UsuarioRol");

                if (usuarioId == null)
                {
                    return Json(new { error = "Sesión expirada." });
                }

                if (rol != "Admin" && rol != "Director")
                {
                    return Json(new { error = "No tiene permisos para realizar esta acción." });
                }

                if (ModelState.IsValid)
                {
                    var omision = await _context.OmisionesMarca.FindAsync(viewModel.OmisionId);
                    if (omision == null)
                    {
                        return Json(new { error = "Omisión no encontrada." });
                    }

                    omision.Estado = viewModel.Resolucion;
                    omision.Resolucion = viewModel.Resolucion;
                    omision.ObservacionesResolucion = viewModel.ObservacionesResolucion;
                    omision.AprobadoPorId = usuarioId.Value;
                    omision.FechaAprobacion = DateTime.Now;
                    omision.ComentariosAprobador = viewModel.ObservacionesResolucion;

                    _context.Update(omision);
                    await _context.SaveChangesAsync();

                    await _auditService.LogActivityAsync(
                        usuarioId.Value,
                        "Resolver",
                        "OmisionMarca",
                        omision.Id,
                        null,
                        $"Omisión resuelta: {viewModel.Resolucion}"
                    );

                    TempData["SuccessMessage"] = "Omisión resuelta correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                // Si hay errores, recargar la omisión para mostrar la vista
                var omisionReload = await _context.OmisionesMarca
                    .Include(o => o.Usuario)
                    .FirstOrDefaultAsync(m => m.Id == viewModel.OmisionId);

                if (omisionReload != null)
                {
                    viewModel.TipoOmision = omisionReload.TipoOmision ?? string.Empty;
                    viewModel.FechaOmision = omisionReload.FechaOmision;
                    viewModel.Motivo = omisionReload.Motivo ?? string.Empty;
                    viewModel.NombreSolicitante = omisionReload.Usuario?.Nombre ?? "No especificado";
                    viewModel.Cedula = omisionReload.Cedula;
                    viewModel.Puesto = omisionReload.Puesto;
                    viewModel.Instancia = omisionReload.Instancia;
                    viewModel.CategoriaPersonal = omisionReload.CategoriaPersonal;
                    viewModel.Titulo = omisionReload.Titulo;

                    ViewBag.Omision = omisionReload;
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al procesar la resolución.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
