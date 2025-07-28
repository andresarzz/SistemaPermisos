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
    public class PermisosController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditService _auditService;
        private readonly IUserService _userService;

        public PermisosController(IUnitOfWork unitOfWork, IAuditService auditService, IUserService userService)
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

            IQueryable<Permiso> permisos;

            if (userRole == "Admin" || userRole == "Supervisor")
            {
                permisos = _unitOfWork.Permisos.GetAll().OrderByDescending(p => p.FechaSolicitud);
            }
            else // Docente
            {
                permisos = _unitOfWork.Permisos.Find(p => p.UsuarioId == int.Parse(userId!)).OrderByDescending(p => p.FechaSolicitud);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                permisos = permisos.Where(p => p.TipoPermiso.Contains(searchString) ||
                                               p.Motivo.Contains(searchString) ||
                                               p.Estado.Contains(searchString));
            }

            var paginatedList = await PaginatedList<Permiso>.CreateAsync(permisos.AsNoTracking(), pageNumber, pageSize);

            // Eager load related user for display
            foreach (var permiso in paginatedList)
            {
                permiso.Usuario = await _userService.GetUserById(permiso.UsuarioId);
                if (permiso.AprobadoPorId.HasValue)
                {
                    permiso.AprobadoPor = await _userService.GetUserById(permiso.AprobadoPorId.Value);
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
        public async Task<IActionResult> Create(PermisoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    ModelState.AddModelError(string.Empty, "Usuario no autenticado.");
                    return View(model);
                }

                string? documentPath = null;
                if (model.DocumentoAdjunto != null)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "permisos");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.DocumentoAdjunto.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.DocumentoAdjunto.CopyToAsync(fileStream);
                    }
                    documentPath = Path.Combine("/uploads", "permisos", uniqueFileName);
                }

                var permiso = new Permiso
                {
                    UsuarioId = int.Parse(userId),
                    TipoPermiso = model.TipoPermiso,
                    FechaInicio = model.FechaInicio,
                    FechaFin = model.FechaFin,
                    HoraDesde = model.HoraDesde,
                    HoraHasta = model.HoraHasta,
                    JornadaCompleta = model.JornadaCompleta,
                    MediaJornada = model.MediaJornada,
                    CantidadLecciones = model.CantidadLecciones,
                    Cedula = model.Cedula,
                    Puesto = model.Puesto,
                    Condicion = model.Condicion,
                    TipoMotivo = model.TipoMotivo,
                    TipoConvocatoria = model.TipoConvocatoria,
                    Motivo = model.Motivo,
                    DocumentoAdjunto = documentPath,
                    FechaSolicitud = DateTime.Now,
                    Estado = "Pendiente"
                };

                await _unitOfWork.Permisos.AddAsync(permiso);
                await _unitOfWork.SaveAsync();

                await _auditService.LogAction(int.Parse(userId), "Crear Permiso", $"Permiso de tipo '{permiso.TipoPermiso}' creado (ID: {permiso.Id}).");
                TempData["SuccessMessage"] = "Permiso solicitado exitosamente.";
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

            var permiso = await _unitOfWork.Permisos.GetByIdAsync(id.Value);
            if (permiso == null)
            {
                return NotFound();
            }

            permiso.Usuario = await _userService.GetUserById(permiso.UsuarioId);
            if (permiso.AprobadoPorId.HasValue)
            {
                permiso.AprobadoPor = await _userService.GetUserById(permiso.AprobadoPorId.Value);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = await _userService.GetUserRole(int.Parse(userId!));

            if (userRole == "Docente" && permiso.UsuarioId != int.Parse(userId!))
            {
                return Forbid(); // Docentes can only view their own permissions
            }

            return View(permiso);
        }

        [HttpGet]
        [Authorize(Policy = "SupervisorPolicy")]
        public async Task<IActionResult> Resolve(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permiso = await _unitOfWork.Permisos.GetByIdAsync(id.Value);
            if (permiso == null)
            {
                return NotFound();
            }

            var model = new ResolucionPermisoViewModel
            {
                PermisoId = permiso.Id,
                NuevoEstado = permiso.Estado,
                ComentariosAprobador = permiso.ComentariosAprobador,
                TipoRebajo = permiso.TipoRebajo
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SupervisorPolicy")]
        public async Task<IActionResult> Resolve(ResolucionPermisoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var permiso = await _unitOfWork.Permisos.GetByIdAsync(model.PermisoId);
                if (permiso == null)
                {
                    return NotFound();
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    ModelState.AddModelError(string.Empty, "Usuario no autenticado.");
                    return View(model);
                }

                permiso.Estado = model.NuevoEstado;
                permiso.ComentariosAprobador = model.ComentariosAprobador;
                permiso.TipoRebajo = model.TipoRebajo;
                permiso.FechaResolucion = DateTime.Now;
                permiso.AprobadoPorId = int.Parse(userId);

                _unitOfWork.Permisos.Update(permiso);
                await _unitOfWork.SaveAsync();

                await _auditService.LogAction(int.Parse(userId), "Resolver Permiso", $"Permiso (ID: {permiso.Id}) resuelto a estado '{permiso.Estado}'.");
                TempData["SuccessMessage"] = "Permiso resuelto exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Justify(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permiso = await _unitOfWork.Permisos.GetByIdAsync(id.Value);
            if (permiso == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (permiso.UsuarioId != int.Parse(userId!))
            {
                return Forbid(); // Only the owner can justify
            }

            if (permiso.Estado != "Rechazado" && permiso.Estado != "Pendiente")
            {
                TempData["ErrorMessage"] = "Solo se pueden justificar permisos en estado 'Pendiente' o 'Rechazado'.";
                return RedirectToAction(nameof(Details), new { id = permiso.Id });
            }

            var model = new JustificarPermisoViewModel
            {
                PermisoId = permiso.Id,
                Motivo = permiso.Justificacion ?? string.Empty
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Justify(JustificarPermisoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var permiso = await _unitOfWork.Permisos.GetByIdAsync(model.PermisoId);
                if (permiso == null)
                {
                    return NotFound();
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (permiso.UsuarioId != int.Parse(userId!))
                {
                    return Forbid();
                }

                if (permiso.Estado != "Rechazado" && permiso.Estado != "Pendiente")
                {
                    ModelState.AddModelError(string.Empty, "Solo se pueden justificar permisos en estado 'Pendiente' o 'Rechazado'.");
                    return View(model);
                }

                string? documentPath = null;
                if (model.DocumentoJustificacion != null)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "justificaciones");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.DocumentoJustificacion.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.DocumentoJustificacion.CopyToAsync(fileStream);
                    }
                    documentPath = Path.Combine("/uploads", "justificaciones", uniqueFileName);
                }

                permiso.Justificacion = model.Motivo;
                permiso.DocumentoAdjunto = documentPath; // Reutilizamos este campo para la justificación
                permiso.FechaJustificacion = DateTime.Now;
                permiso.Estado = "Justificado"; // Change state to Justificado

                _unitOfWork.Permisos.Update(permiso);
                await _unitOfWork.SaveAsync();

                await _auditService.LogAction(int.Parse(userId), "Justificar Permiso", $"Permiso (ID: {permiso.Id}) justificado.");
                TempData["SuccessMessage"] = "Permiso justificado exitosamente.";
                return RedirectToAction(nameof(Details), new { id = permiso.Id });
            }
            return View(model);
        }
    }
}
