using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaPermisos.Models;
using SistemaPermisos.Services;
using SistemaPermisos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace SistemaPermisos.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuditService _auditService;
        private readonly IExportService _exportService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public UsersController(
            IUserService userService,
            IAuditService auditService,
            IExportService exportService,
            IWebHostEnvironment hostEnvironment)
        {
            _userService = userService;
            _auditService = auditService;
            _exportService = exportService;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Users
        public async Task<IActionResult> Index(string searchString, string roleFilter, string sortOrder, int page = 1)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["RoleFilter"] = roleFilter;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["RoleSortParm"] = sortOrder == "Role" ? "role_desc" : "Role";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            // Obtener todos los usuarios
            var allUsers = await _userService.GetAllUsersAsync();

            // Aplicar filtros
            var filteredUsers = allUsers;

            if (!string.IsNullOrEmpty(searchString))
            {
                filteredUsers = filteredUsers.Where(u =>
                    u.Nombre.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    u.Correo.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    (u.Cedula != null && u.Cedula.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (!string.IsNullOrEmpty(roleFilter))
            {
                filteredUsers = filteredUsers.Where(u => u.Rol == roleFilter);
            }

            // Aplicar ordenamiento
            filteredUsers = sortOrder switch
            {
                "name_desc" => filteredUsers.OrderByDescending(u => u.Nombre),
                "Role" => filteredUsers.OrderBy(u => u.Rol),
                "role_desc" => filteredUsers.OrderByDescending(u => u.Rol),
                "Date" => filteredUsers.OrderBy(u => u.FechaRegistro),
                "date_desc" => filteredUsers.OrderByDescending(u => u.FechaRegistro),
                _ => filteredUsers.OrderBy(u => u.Nombre),
            };

            // Paginación
            int pageSize = 10;
            int totalItems = filteredUsers.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            page = Math.Max(1, Math.Min(page, totalPages));

            var pagedUsers = filteredUsers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;

            // Registrar en auditoría
            await _auditService.LogActivityAsync(
                HttpContext.Session.GetInt32("UsuarioId"),
                "Consultar",
                "Usuario",
                null,
                null,
                $"Filtros: {searchString}, {roleFilter}, {sortOrder}"
            );

            return View(pagedUsers);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var usuario = await _userService.GetUserByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Obtener permisos del usuario
            var permisos = await _userService.GetUserPermissionsAsync(id);
            ViewBag.Permisos = permisos;

            // Obtener actividad reciente del usuario
            var actividad = await _auditService.GetUserActivityAsync(id, 10);
            ViewBag.Actividad = actividad;

            // Registrar en auditoría
            await _auditService.LogActivityAsync(
                HttpContext.Session.GetInt32("UsuarioId"),
                "Ver Detalles",
                "Usuarios",
                id
            );

            return View(usuario);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                if (await _userService.CreateUserAsync(model))
                {
                    TempData["SuccessMessage"] = "Usuario creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Correo", "Este correo ya está registrado");
                }
            }
            return View(model);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var usuario = await _userService.GetUserByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var viewModel = new UserEditViewModel
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Rol = usuario.Rol,
                Cedula = usuario.Cedula,
                Puesto = usuario.Puesto,
                Telefono = usuario.Telefono,
                Departamento = usuario.Departamento,
                Direccion = usuario.Direccion,
                FechaRegistro = usuario.FechaRegistro
            };

            return View(viewModel);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserEditViewModel model)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (await _userService.UpdateUserAsync(model))
                {
                    TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Correo", "Este correo ya está registrado por otro usuario");
                }
            }
            return View(model);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // No permitir eliminar al usuario actual
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == id)
            {
                TempData["ErrorMessage"] = "No puedes eliminar tu propio usuario.";
                return RedirectToAction(nameof(Index));
            }

            var usuario = await _userService.GetUserByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // No permitir eliminar al usuario actual
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == id)
            {
                TempData["ErrorMessage"] = "No puedes eliminar tu propio usuario.";
                return RedirectToAction(nameof(Index));
            }

            if (await _userService.DeleteUserAsync(id))
            {
                TempData["SuccessMessage"] = "Usuario eliminado correctamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "No se puede eliminar el usuario porque tiene registros asociados.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Users/ChangeRole/5
        public async Task<IActionResult> ChangeRole(int id)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // No permitir cambiar el rol del usuario actual
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == id)
            {
                TempData["ErrorMessage"] = "No puedes cambiar tu propio rol.";
                return RedirectToAction(nameof(Index));
            }

            var usuario = await _userService.GetUserByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var viewModel = new ChangeRoleViewModel
            {
                UsuarioId = usuario.Id,
                Nombre = usuario.Nombre,
                RolActual = usuario.Rol,
                NuevoRol = usuario.Rol
            };

            return View(viewModel);
        }

        // POST: Users/ChangeRole/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(ChangeRoleViewModel model)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // No permitir cambiar el rol del usuario actual
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == model.UsuarioId)
            {
                TempData["ErrorMessage"] = "No puedes cambiar tu propio rol.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                if (await _userService.ChangeUserRoleAsync(model))
                {
                    TempData["SuccessMessage"] = "Rol actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(model);
        }

        // GET: Users/ToggleStatus/5
        public async Task<IActionResult> ToggleStatus(int id)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // No permitir bloquear al usuario actual
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == id)
            {
                TempData["ErrorMessage"] = "No puedes bloquear tu propia cuenta.";
                return RedirectToAction(nameof(Index));
            }

            if (await _userService.ToggleUserStatusAsync(id))
            {
                var usuario = await _userService.GetUserByIdAsync(id);
                TempData["SuccessMessage"] = usuario.Activo
                    ? "Usuario activado correctamente."
                    : "Usuario bloqueado correctamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo cambiar el estado del usuario.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Users/ManagePermissions/5
        public async Task<IActionResult> ManagePermissions(int id)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var usuario = await _userService.GetUserByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Obtener permisos actuales del usuario
            var permisos = await _userService.GetUserPermissionsAsync(id);

            var viewModel = new ManagePermissionsViewModel
            {
                UsuarioId = usuario.Id,
                Nombre = usuario.Nombre,
                Permisos = permisos.ToList()
            };

            return View(viewModel);
        }

        // POST: Users/ManagePermissions/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManagePermissions(ManagePermissionsViewModel model)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                // Asegurarse de que la colección de permisos no sea nula
                if (model.Permisos == null)
                {
                    model.Permisos = new List<string>();
                }

                // Obtener permisos actuales
                var permisosActuales = (await _userService.GetUserPermissionsAsync(model.UsuarioId)).ToList();

                // Permisos a agregar (están en el modelo pero no en los actuales)
                foreach (var permiso in model.Permisos.Where(p => !permisosActuales.Contains(p)))
                {
                    await _userService.AddPermissionAsync(model.UsuarioId, permiso);
                }

                // Permisos a eliminar (están en los actuales pero no en el modelo)
                foreach (var permiso in permisosActuales.Where(p => !model.Permisos.Contains(p)))
                {
                    await _userService.RemovePermissionAsync(model.UsuarioId, permiso);
                }

                TempData["SuccessMessage"] = "Permisos actualizados correctamente.";
                return RedirectToAction(nameof(Details), new { id = model.UsuarioId });
            }

            return View(model);
        }

        // GET: Users/ExportToExcel
        public async Task<IActionResult> ExportToExcel(string searchString, string roleFilter, string sortOrder)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // Obtener todos los usuarios
            var allUsers = await _userService.GetAllUsersAsync();

            // Aplicar filtros
            var filteredUsers = allUsers;

            if (!string.IsNullOrEmpty(searchString))
            {
                filteredUsers = filteredUsers.Where(u =>
                    u.Nombre.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    u.Correo.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    (u.Cedula != null && u.Cedula.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (!string.IsNullOrEmpty(roleFilter))
            {
                filteredUsers = filteredUsers.Where(u => u.Rol == roleFilter);
            }

            // Aplicar ordenamiento
            filteredUsers = sortOrder switch
            {
                "name_desc" => filteredUsers.OrderByDescending(u => u.Nombre),
                "Role" => filteredUsers.OrderBy(u => u.Rol),
                "role_desc" => filteredUsers.OrderByDescending(u => u.Rol),
                "Date" => filteredUsers.OrderBy(u => u.FechaRegistro),
                "date_desc" => filteredUsers.OrderByDescending(u => u.FechaRegistro),
                _ => filteredUsers.OrderBy(u => u.Nombre),
            };

            // Exportar a Excel
            var excelBytes = await _exportService.ExportUsersToExcelAsync(filteredUsers);

            // Registrar en auditoría
            await _auditService.LogActivityAsync(
                HttpContext.Session.GetInt32("UsuarioId"),
                "Exportar a Excel",
                "Usuarios",
                null,
                null,
                $"Filtros: {searchString}, {roleFilter}, {sortOrder}"
            );

            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Usuarios.xlsx");
        }

        // GET: Users/ExportToPdf
        public async Task<IActionResult> ExportToPdf(string searchString, string roleFilter, string sortOrder)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // Obtener todos los usuarios
            var allUsers = await _userService.GetAllUsersAsync();

            // Aplicar filtros
            var filteredUsers = allUsers;

            if (!string.IsNullOrEmpty(searchString))
            {
                filteredUsers = filteredUsers.Where(u =>
                    u.Nombre.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    u.Correo.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    (u.Cedula != null && u.Cedula.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (!string.IsNullOrEmpty(roleFilter))
            {
                filteredUsers = filteredUsers.Where(u => u.Rol == roleFilter);
            }

            // Aplicar ordenamiento
            filteredUsers = sortOrder switch
            {
                "name_desc" => filteredUsers.OrderByDescending(u => u.Nombre),
                "Role" => filteredUsers.OrderBy(u => u.Rol),
                "role_desc" => filteredUsers.OrderByDescending(u => u.Rol),
                "Date" => filteredUsers.OrderBy(u => u.FechaRegistro),
                "date_desc" => filteredUsers.OrderByDescending(u => u.FechaRegistro),
                _ => filteredUsers.OrderBy(u => u.Nombre),
            };

            // Exportar a PDF
            var pdfBytes = await _exportService.ExportUsersToPdfAsync(filteredUsers);

            // Registrar en auditoría
            await _auditService.LogActivityAsync(
                HttpContext.Session.GetInt32("UsuarioId"),
                "Exportar a PDF",
                "Usuarios",
                null,
                null,
                $"Filtros: {searchString}, {roleFilter}, {sortOrder}"
            );

            return File(pdfBytes, "application/pdf", "Usuarios.pdf");
        }

        // GET: Users/AuditLog
        public async Task<IActionResult> AuditLog(string searchString, string actionFilter, string tableFilter, int page = 1)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["ActionFilter"] = actionFilter;
            ViewData["TableFilter"] = tableFilter;

            // Obtener todos los registros de auditoría
            var allLogs = await _auditService.GetAllActivityAsync(1000);

            // Aplicar filtros
            var filteredLogs = allLogs;

            if (!string.IsNullOrEmpty(searchString))
            {
                filteredLogs = filteredLogs.Where(l =>
                    (l.Usuario != null && l.Usuario.Nombre.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                    l.DireccionIP.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    (l.DatosAntiguos != null && l.DatosAntiguos.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                    (l.DatosNuevos != null && l.DatosNuevos.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (!string.IsNullOrEmpty(actionFilter))
            {
                filteredLogs = filteredLogs.Where(l => l.Accion == actionFilter);
            }

            if (!string.IsNullOrEmpty(tableFilter))
            {
                filteredLogs = filteredLogs.Where(l => l.Tabla == tableFilter);
            }

            // Ordenar por fecha descendente
            filteredLogs = filteredLogs.OrderByDescending(l => l.Fecha);

            // Paginación
            int pageSize = 20;
            int totalItems = filteredLogs.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            page = Math.Max(1, Math.Min(page, totalPages));

            var pagedLogs = filteredLogs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;

            // Obtener listas de acciones y tablas para los filtros
            ViewBag.Acciones = allLogs.Select(l => l.Accion).Distinct().OrderBy(a => a).ToList();
            ViewBag.Tablas = allLogs.Select(l => l.Tabla).Distinct().OrderBy(t => t).ToList();

            return View(pagedLogs);
        }

        // GET: Users/ExportAuditLogToExcel
        public async Task<IActionResult> ExportAuditLogToExcel(string searchString, string actionFilter, string tableFilter)
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // Obtener todos los registros de auditoría
            var allLogs = await _auditService.GetAllActivityAsync(1000);

            // Aplicar filtros
            var filteredLogs = allLogs;

            if (!string.IsNullOrEmpty(searchString))
            {
                filteredLogs = filteredLogs.Where(l =>
                    (l.Usuario != null && l.Usuario.Nombre.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                    l.DireccionIP.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    (l.DatosAntiguos != null && l.DatosAntiguos.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                    (l.DatosNuevos != null && l.DatosNuevos.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (!string.IsNullOrEmpty(actionFilter))
            {
                filteredLogs = filteredLogs.Where(l => l.Accion == actionFilter);
            }

            if (!string.IsNullOrEmpty(tableFilter))
            {
                filteredLogs = filteredLogs.Where(l => l.Tabla == tableFilter);
            }

            // Ordenar por fecha descendente
            filteredLogs = filteredLogs.OrderByDescending(l => l.Fecha);

            // Exportar a Excel
            var excelBytes = await _exportService.ExportAuditLogToExcelAsync(filteredLogs);

            // Registrar en auditoría
            await _auditService.LogActivityAsync(
                HttpContext.Session.GetInt32("UsuarioId"),
                "Exportar a Excel",
                "AuditLogs",
                null,
                null,
                $"Filtros: {searchString}, {actionFilter}, {tableFilter}"
            );

            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Auditoria.xlsx");
        }
    }
}
