using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaPermisos.Models;
using SistemaPermisos.Services;
using SistemaPermisos.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SistemaPermisos.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuditService _auditService;
        private readonly IExportService _exportService;

        public UsersController(IUserService userService, IAuditService auditService, IExportService exportService)
        {
            _userService = userService;
            _auditService = auditService;
            _exportService = exportService;
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

            var users = await _userService.GetPaginatedUsers(pageNumber, pageSize, searchString, currentFilter);
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = await _userService.GetAllPermissions();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.CreateUser(model);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, result.Message);
            }
            ViewBag.Roles = await _userService.GetAllPermissions();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserById(id.Value);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserEditViewModel
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Apellidos = user.Apellidos,
                NombreUsuario = user.NombreUsuario,
                Email = user.Email,
                Rol = user.Rol,
                Cedula = user.Cedula,
                Puesto = user.Puesto,
                Telefono = user.Telefono,
                Departamento = user.Departamento,
                Direccion = user.Direccion,
                Activo = user.IsActive,
                FechaRegistro = user.FechaRegistro,
                FechaNacimiento = user.FechaNacimiento,
                FotoPerfilActual = user.FotoPerfilUrl
            };

            ViewBag.Roles = await _userService.GetAllPermissions();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.UpdateUser(model);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, result.Message);
            }
            ViewBag.Roles = await _userService.GetAllPermissions();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserById(id.Value);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileViewModel
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Apellidos = user.Apellidos,
                NombreUsuario = user.NombreUsuario,
                Email = user.Email,
                Rol = user.Rol,
                Cedula = user.Cedula,
                Puesto = user.Puesto,
                Telefono = user.Telefono,
                Departamento = user.Departamento,
                Direccion = user.Direccion,
                FechaNacimiento = user.FechaNacimiento,
                FechaRegistro = user.FechaRegistro,
                UltimoAcceso = user.UltimoAcceso,
                Activo = user.IsActive,
                FotoPerfilUrl = user.FotoPerfilUrl
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserById(id.Value);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _userService.DeleteUser(id);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangeRole(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserById(id.Value);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ChangeRoleViewModel
            {
                UserId = user.Id,
                NewRole = user.Rol
            };

            ViewBag.Roles = await _userService.GetAllPermissions();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(ChangeRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ChangeUserRole(model);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, result.Message);
            }
            ViewBag.Roles = await _userService.GetAllPermissions();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManagePermissions(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserById(id.Value);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ManagePermissionsViewModel
            {
                UserId = user.Id,
                UserName = user.NombreUsuario,
                UserPermissions = await _userService.GetUserPermissions(user.Id),
                AllPermissions = await _userService.GetAllPermissions(),
                SelectedPermissions = await _userService.GetUserPermissions(user.Id) // Pre-select current role
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManagePermissions(ManagePermissionsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ManagePermissions(model);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, result.Message);
            }
            var user = await _userService.GetUserById(model.UserId);
            if (user != null)
            {
                model.UserName = user.NombreUsuario;
                model.UserPermissions = await _userService.GetUserPermissions(user.Id);
            }
            model.AllPermissions = await _userService.GetAllPermissions();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AuditLog(int pageNumber = 1, int pageSize = 10)
        {
            var auditLogs = _unitOfWork.AuditLogs.GetAll().OrderByDescending(a => a.FechaHora);
            var paginatedList = await PaginatedList<AuditLog>.CreateAsync(auditLogs.AsNoTracking(), pageNumber, pageSize);

            // Eager load related user for display
            foreach (var log in paginatedList)
            {
                log.Usuario = await _userService.GetUserById(log.UsuarioId);
            }

            return View(paginatedList);
        }

        [HttpGet]
        public async Task<IActionResult> ExportUsers()
        {
            var users = await _userService.GetPaginatedUsers(1, int.MaxValue, null, null); // Get all users
            var fileContents = _exportService.ExportUsersToExcel(users.ToList());
            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Usuarios.xlsx");
        }
    }
}
