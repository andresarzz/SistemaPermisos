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
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public UsersController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            // Verificar que sea administrador
            var rol = HttpContext.Session.GetString("UsuarioRol");
            if (rol != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var usuarios = await _context.Usuarios.ToListAsync();
            return View(usuarios);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
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

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

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
                // Verificar si el correo ya existe
                if (_context.Usuarios.Any(u => u.Correo == model.Correo))
                {
                    ModelState.AddModelError("Correo", "Este correo ya está registrado");
                    return View(model);
                }

                var usuario = new Usuario
                {
                    Nombre = model.Nombre,
                    Correo = model.Correo,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Rol = model.Rol,
                    Cedula = model.Cedula,
                    Puesto = model.Puesto,
                    Telefono = model.Telefono,
                    Departamento = model.Departamento,
                    FechaRegistro = DateTime.Now,
                    UltimaActualizacion = DateTime.Now
                };

                _context.Add(usuario);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Usuario creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

            var usuario = await _context.Usuarios.FindAsync(id);
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
                try
                {
                    var usuario = await _context.Usuarios.FindAsync(id);
                    if (usuario == null)
                    {
                        return NotFound();
                    }

                    // Verificar si el correo ya existe en otro usuario
                    if (usuario.Correo != model.Correo && _context.Usuarios.Any(u => u.Correo == model.Correo))
                    {
                        ModelState.AddModelError("Correo", "Este correo ya está registrado por otro usuario");
                        return View(model);
                    }

                    usuario.Nombre = model.Nombre;
                    usuario.Correo = model.Correo;
                    usuario.Rol = model.Rol;
                    usuario.Cedula = model.Cedula;
                    usuario.Puesto = model.Puesto;
                    usuario.Telefono = model.Telefono;
                    usuario.Departamento = model.Departamento;
                    usuario.Direccion = model.Direccion;
                    usuario.UltimaActualizacion = DateTime.Now;

                    // Si se proporciona una nueva contraseña, actualizarla
                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        usuario.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                    }

                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            // No permitir eliminar al usuario actual
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == id)
            {
                TempData["ErrorMessage"] = "No puedes eliminar tu propio usuario.";
                return RedirectToAction(nameof(Index));
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

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                // Verificar si el usuario tiene permisos, omisiones o reportes
                var tienePermisos = await _context.Permisos.AnyAsync(p => p.UsuarioId == id);
                var tieneOmisiones = await _context.OmisionesMarca.AnyAsync(o => o.UsuarioId == id);
                var tieneReportes = await _context.ReportesDanos.AnyAsync(r => r.UsuarioId == id);

                if (tienePermisos || tieneOmisiones || tieneReportes)
                {
                    TempData["ErrorMessage"] = "No se puede eliminar el usuario porque tiene registros asociados.";
                    return RedirectToAction(nameof(Index));
                }

                // Eliminar la foto de perfil si existe
                if (!string.IsNullOrEmpty(usuario.FotoPerfil))
                {
                    var filePath = Path.Combine(_hostEnvironment.WebRootPath, usuario.FotoPerfil.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Usuario eliminado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Users/ChangeRole/5
        public async Task<IActionResult> ChangeRole(int? id)
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

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // No permitir cambiar el rol del usuario actual
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == id)
            {
                TempData["ErrorMessage"] = "No puedes cambiar tu propio rol.";
                return RedirectToAction(nameof(Index));
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
                var usuario = await _context.Usuarios.FindAsync(model.UsuarioId);
                if (usuario == null)
                {
                    return NotFound();
                }

                usuario.Rol = model.NuevoRol;
                usuario.UltimaActualizacion = DateTime.Now;

                _context.Update(usuario);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Rol actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}

