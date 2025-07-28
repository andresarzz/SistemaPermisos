using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Models;
using SistemaPermisos.Repositories;
using SistemaPermisos.Services;
using SistemaPermisos.ViewModels;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SistemaPermisos.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditService _auditService;
        private readonly IUserService _userService;

        public ReportesController(IUnitOfWork unitOfWork, IAuditService auditService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _auditService = auditService;
            _userService = userService;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string? searchString = null, string? currentFilter = null)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = await _userService.GetUserRole(int.Parse(userId!));

            IQueryable<ReporteDano> reportes;

            if (userRole == "Admin" || userRole == "Supervisor")
            {
                reportes = _unitOfWork.ReportesDano.GetAll().OrderByDescending(r => r.FechaReporte);
            }
            else // Docente
            {
                reportes = _unitOfWork.ReportesDano.Find(r => r.UsuarioId == int.Parse(userId!)).OrderByDescending(r => r.FechaReporte);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                reportes = reportes.Where(r => r.TipoDano.Contains(searchString) ||
                                                r.Descripcion.Contains(searchString) ||
                                                r.Equipo.Contains(searchString) ||
                                                r.Ubicacion!.Contains(searchString) ||
                                                r.Estado.Contains(searchString));
            }

            var paginatedList = await PaginatedList<ReporteDano>.CreateAsync(reportes.AsNoTracking(), pageNumber, pageSize);

            // Eager load related user for display
            foreach (var reporte in paginatedList)
            {
                reporte.Usuario = await _userService.GetUserById(reporte.UsuarioId);
                if (reporte.ResueltoPorId.HasValue)
                {
                    reporte.ResueltoPor = await _userService.GetUserById(reporte.ResueltoPorId.Value);
                }
            }

            return View(paginatedList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReporteViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    ModelState.AddModelError(string.Empty, "Usuario no autenticado.");
                    return View(model);
                }

                string? evidenciaPath = null;
                if (model.Evidencia != null)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "reportes");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Evidencia.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Evidencia.CopyToAsync(fileStream);
                    }
                    evidenciaPath = Path.Combine("/uploads", "reportes", uniqueFileName);
                }

                var reporte = new ReporteDano
                {
                    UsuarioId = int.Parse(userId),
                    TipoDano = model.TipoDano,
                    Descripcion = model.Descripcion,
                    FechaReporte = model.FechaReporte,
                    Equipo = model.Equipo,
                    Ubicacion = model.Ubicacion,
                    Observaciones = model.Observaciones,
                    RutaEvidencia = evidenciaPath,
                    Estado = "Pendiente"
                };

                await _unitOfWork.ReportesDano.AddAsync(reporte);
                await _unitOfWork.SaveAsync();

                await _auditService.LogAction(int.Parse(userId), "Crear Reporte de Daño", $"Reporte de daño '{reporte.TipoDano}' creado (ID: {reporte.Id}).");
                TempData["SuccessMessage"] = "Reporte de daño creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reporte = await _unitOfWork.ReportesDano.GetByIdAsync(id.Value);
            if (reporte == null)
            {
                return NotFound();
            }

            reporte.Usuario = await _userService.GetUserById(reporte.UsuarioId);
            if (reporte.ResueltoPorId.HasValue)
            {
                reporte.ResueltoPor = await _userService.GetUserById(reporte.ResueltoPorId.Value);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = await _userService.GetUserRole(int.Parse(userId!));

            if (userRole == "Docente" && reporte.UsuarioId != int.Parse(userId!))
            {
                return Forbid(); // Docentes can only view their own reports
            }

            return View(reporte);
        }

        [HttpGet]
        [Authorize(Policy = "SupervisorPolicy")]
        public async Task<IActionResult> Resolve(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reporte = await _unitOfWork.ReportesDano.GetByIdAsync(id.Value);
            if (reporte == null)
            {
                return NotFound();
            }

            var model = new ResolucionReporteViewModel
            {
                ReporteId = reporte.Id,
                NuevoEstado = reporte.Estado,
                ComentariosResolucion = reporte.ObservacionesResolucion
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SupervisorPolicy")]
        public async Task<IActionResult> Resolve(ResolucionReporteViewModel model)
        {
            if (ModelState.IsValid)
            {
                var reporte = await _unitOfWork.ReportesDano.GetByIdAsync(model.ReporteId);
                if (reporte == null)
                {
                    return NotFound();
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    ModelState.AddModelError(string.Empty, "Usuario no autenticado.");
                    return View(model);
                }

                reporte.Estado = model.NuevoEstado;
                reporte.ObservacionesResolucion = model.ComentariosResolucion;
                reporte.FechaResolucion = DateTime.Now;
                reporte.ResueltoPorId = int.Parse(userId);

                _unitOfWork.ReportesDano.Update(reporte);
                await _unitOfWork.SaveAsync();

                await _auditService.LogAction(int.Parse(userId), "Resolver Reporte de Daño", $"Reporte de daño (ID: {reporte.Id}) resuelto a estado '{reporte.Estado}'.");
                TempData["SuccessMessage"] = "Reporte de daño resuelto exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}
