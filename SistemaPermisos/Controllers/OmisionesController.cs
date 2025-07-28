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
    public class OmisionesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditService _auditService;
        private readonly IUserService _userService;

        public OmisionesController(IUnitOfWork unitOfWork, IAuditService auditService, IUserService userService)
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

            IQueryable<OmisionMarca> omisiones;

            if (userRole == "Admin" || userRole == "Supervisor")
            {
                omisiones = _unitOfWork.OmisionesMarca.GetAll().OrderByDescending(o => o.FechaSolicitud);
            }
            else // Docente
            {
                omisiones = _unitOfWork.OmisionesMarca.Find(o => o.UsuarioId == int.Parse(userId!)).OrderByDescending(o => o.FechaSolicitud);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                omisiones = omisiones.Where(o => o.TipoOmision.Contains(searchString) ||
                                                 o.Motivo.Contains(searchString) ||
                                                 o.Estado.Contains(searchString));
            }

            var paginatedList = await PaginatedList<OmisionMarca>.CreateAsync(omisiones.AsNoTracking(), pageNumber, pageSize);

            // Eager load related user for display
            foreach (var omision in paginatedList)
            {
                omision.Usuario = await _userService.GetUserById(omision.UsuarioId);
                if (omision.AprobadoPorId.HasValue)
                {
                    omision.AprobadoPor = await _userService.GetUserById(omision.AprobadoPorId.Value);
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
        public async Task<IActionResult> Create(OmisionViewModel model)
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
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "omisiones");
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
                    evidenciaPath = Path.Combine("/uploads", "omisiones", uniqueFileName);
                }

                var omision = new OmisionMarca
                {
                    UsuarioId = int.Parse(userId),
                    TipoOmision = model.TipoOmision,
                    FechaOmision = model.FechaOmision,
                    HoraOmision = model.HoraOmision,
                    Motivo = model.Motivo,
                    Cedula = model.Cedula,
                    Puesto = model.Puesto,
                    Instancia = model.Instancia,
                    CategoriaPersonal = model.CategoriaPersonal,
                    Titulo = model.Titulo,
                    RutaEvidencia = evidenciaPath,
                    FechaSolicitud = DateTime.Now,
                    Estado = "Pendiente"
                };

                await _unitOfWork.OmisionesMarca.AddAsync(omision);
                await _unitOfWork.SaveAsync();

                await _auditService.LogAction(int.Parse(userId), "Crear Omisión de Marca", $"Omisión de marca de tipo '{omision.TipoOmision}' creada (ID: {omision.Id}).");
                TempData["SuccessMessage"] = "Omisión de marca solicitada exitosamente.";
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

            var omision = await _unitOfWork.OmisionesMarca.GetByIdAsync(id.Value);
            if (omision == null)
            {
                return NotFound();
            }

            omision.Usuario = await _userService.GetUserById(omision.UsuarioId);
            if (omision.AprobadoPorId.HasValue)
            {
                omision.AprobadoPor = await _userService.GetUserById(omision.AprobadoPorId.Value);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = await _userService.GetUserRole(int.Parse(userId!));

            if (userRole == "Docente" && omision.UsuarioId != int.Parse(userId!))
            {
                return Forbid(); // Docentes can only view their own omisiones
            }

            return View(omision);
        }

        [HttpGet]
        [Authorize(Policy = "SupervisorPolicy")]
        public async Task<IActionResult> Resolve(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var omision = await _unitOfWork.OmisionesMarca.GetByIdAsync(id.Value);
            if (omision == null)
            {
                return NotFound();
            }

            var model = new ResolucionOmisionViewModel
            {
                OmisionId = omision.Id,
                NuevoEstado = omision.Estado,
                ComentariosAprobador = omision.ComentariosAprobador
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SupervisorPolicy")]
        public async Task<IActionResult> Resolve(ResolucionOmisionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var omision = await _unitOfWork.OmisionesMarca.GetByIdAsync(model.OmisionId);
                if (omision == null)
                {
                    return NotFound();
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    ModelState.AddModelError(string.Empty, "Usuario no autenticado.");
                    return View(model);
                }

                omision.Estado = model.NuevoEstado;
                omision.ComentariosAprobador = model.ComentariosAprobador;
                omision.FechaResolucion = DateTime.Now;
                omision.AprobadoPorId = int.Parse(userId);

                _unitOfWork.OmisionesMarca.Update(omision);
                await _unitOfWork.SaveAsync();

                await _auditService.LogAction(int.Parse(userId), "Resolver Omisión de Marca", $"Omisión de marca (ID: {omision.Id}) resuelta a estado '{omision.Estado}'.");
                TempData["SuccessMessage"] = "Omisión de marca resuelta exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}
