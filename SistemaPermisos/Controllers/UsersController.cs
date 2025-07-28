using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaPermisos.Services;
using SistemaPermisos.ViewModels;
using System.Threading.Tasks;
using SistemaPermisos.Models;
using System.Security.Claims;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SistemaPermisos.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuditService _auditService;

        public UsersController(IUserService userService, IAuditService auditService)
        {
            _userService = userService;
            _auditService = auditService;
        }

        public async Task<IActionResult> Index(int? pageNumber)
        {
            int pageSize = 10;
            var users = await _userService.GetAllUsersAsync(pageNumber ?? 1, pageSize);
            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userService.RegisterUserAsync(model);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Message ?? "Error al crear el usuario.");
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserByIdAsync(id.Value);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new UserEditViewModel
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Apellidos = user.Apellidos,
                NombreUsuario = user.NombreUsuario,
                Email = user.Correo,
                Rol = user.Rol,
                Cedula = user.Cedula,
                Puesto = user.Puesto,
                Telefono = user.Telefono,
                Departamento = user.Departamento,
                Direccion = user.Direccion,
                FechaRegistro = user.FechaRegistro,
                Activo = user.Activo,
                FechaNacimiento = user.FechaNacimiento,
                FotoPerfilActual = user.FotoPerfil
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userToUpdate = await _userService.GetUserByIdAsync(model.Id);
            if (userToUpdate == null)
            {
                return NotFound();
            }

            userToUpdate.Nombre = model.Nombre;
            userToUpdate.Apellidos = model.Apellidos ?? string.Empty;
            userToUpdate.NombreUsuario = model.NombreUsuario;
            userToUpdate.Correo = model.Email;
            userToUpdate.Rol = model.Rol;
            userToUpdate.Cedula = model.Cedula;
            userToUpdate.Puesto = model.Puesto;
            userToUpdate.Telefono = model.Telefono;
            userToUpdate.Departamento = model.Departamento;
            userToUpdate.Direccion = model.Direccion;
            userToUpdate.FechaNacimiento = model.FechaNacimiento;
            userToUpdate.Activo = model.Activo;

            var result = await _userService.UpdateUserAsync(userToUpdate);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Usuario actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Message ?? "Error al actualizar el usuario.");
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserByIdAsync(id.Value);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserByIdAsync(id.Value);
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
            var result = await _userService.DeleteUserAsync(id);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Message ?? "Error al eliminar el usuario.");
            return View(await _userService.GetUserByIdAsync(id)); // Return to view with error
        }

        public async Task<IActionResult> ChangeRole(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserByIdAsync(id.Value);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new ChangeRoleViewModel
            {
                UserId = user.Id,
                NewRole = user.Rol
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(ChangeRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userService.ChangeUserRoleAsync(model.UserId, model.NewRole);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Message ?? "Error al cambiar el rol del usuario.");
            return View(model);
        }

        public async Task<IActionResult> ManagePermissions(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserByIdAsync(id.Value);
            if (user == null)
            {
                return NotFound();
            }

            var userPermissions = await _userService.GetUserPermissionsAsync(id.Value);
            var allPermissions = await _userService.GetAllAvailablePermissionsAsync();

            var viewModel = new ManagePermissionsViewModel
            {
                UserId = user.Id,
                UserName = user.NombreUsuario ?? user.Nombre,
                UserPermissions = userPermissions,
                AllPermissions = allPermissions
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManagePermissions(ManagePermissionsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // If SelectedPermissions is null due to no checkboxes being checked, initialize it
                model.SelectedPermissions ??= new List<string>();
                model.AllPermissions = await _userService.GetAllAvailablePermissionsAsync(); // Re-populate AllPermissions for view
                return View(model);
            }

            var result = await _userService.ManageUserPermissionsAsync(model.UserId, model.SelectedPermissions ?? new List<string>());
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Message ?? "Error al gestionar los permisos.");
            model.AllPermissions = await _userService.GetAllAvailablePermissionsAsync(); // Re-populate AllPermissions for view
            return View(model);
        }

        public async Task<IActionResult> AuditLog(int? pageNumber)
        {
            int pageSize = 10;
            var auditLogs = await _auditService.GetAllAuditLogsAsync(pageNumber ?? 1, pageSize);
            return View(auditLogs);
        }
    }
}
