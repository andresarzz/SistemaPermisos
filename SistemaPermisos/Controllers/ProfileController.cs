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
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProfileController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Profile
        public async Task<IActionResult> Index()
        {
            // Verificar si el usuario está autenticado
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound();
            }

            var viewModel = new ProfileViewModel
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Telefono = usuario.Telefono,
                Departamento = usuario.Departamento,
                FechaNacimiento = usuario.FechaNacimiento,
                Direccion = usuario.Direccion,
                FotoPerfilActual = usuario.FotoPerfil,
                FechaRegistro = usuario.FechaRegistro,
                Rol = usuario.Rol
            };

            return View(viewModel);
        }

        // GET: Profile/Edit
        public async Task<IActionResult> Edit()
        {
            // Verificar si el usuario está autenticado
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound();
            }

            var viewModel = new ProfileViewModel
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Telefono = usuario.Telefono,
                Departamento = usuario.Departamento,
                FechaNacimiento = usuario.FechaNacimiento,
                Direccion = usuario.Direccion,
                FotoPerfilActual = usuario.FotoPerfil,
                FechaRegistro = usuario.FechaRegistro,
                Rol = usuario.Rol
            };

            return View(viewModel);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            // Verificar si el usuario está autenticado
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null || usuarioId != model.Id)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var usuario = await _context.Usuarios.FindAsync(usuarioId);
                    if (usuario == null)
                    {
                        return NotFound();
                    }

                    // Actualizar datos del usuario
                    usuario.Nombre = model.Nombre;
                    usuario.Correo = model.Correo;
                    usuario.Telefono = model.Telefono;
                    usuario.Departamento = model.Departamento;
                    usuario.FechaNacimiento = model.FechaNacimiento;
                    usuario.Direccion = model.Direccion;
                    usuario.UltimaActualizacion = DateTime.Now;

                    // Procesar la foto de perfil si se ha subido una nueva
                    if (model.FotoPerfilFile != null && model.FotoPerfilFile.Length > 0)
                    {
                        // Eliminar la foto anterior si existe
                        if (!string.IsNullOrEmpty(usuario.FotoPerfil))
                        {
                            var oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, usuario.FotoPerfil.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Guardar la nueva foto
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "perfiles");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.FotoPerfilFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Asegurar que el directorio existe
                        Directory.CreateDirectory(uploadsFolder);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.FotoPerfilFile.CopyToAsync(fileStream);
                        }

                        usuario.FotoPerfil = "/uploads/perfiles/" + uniqueFileName;

                        // Actualizar la sesión con el nuevo nombre si cambió
                        HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
                    }

                    _context.Update(usuario);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Perfil actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
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
            }

            return View(model);
        }

        // GET: Profile/ChangePassword
        public IActionResult ChangePassword()
        {
            // Verificar si el usuario está autenticado
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // POST: Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            // Verificar si el usuario está autenticado
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var usuario = await _context.Usuarios.FindAsync(usuarioId);
                if (usuario == null)
                {
                    return NotFound();
                }

                // Verificar la contraseña actual
                if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, usuario.Password))
                {
                    ModelState.AddModelError("CurrentPassword", "La contraseña actual es incorrecta.");
                    return View(model);
                }

                // Actualizar la contraseña
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                usuario.UltimaActualizacion = DateTime.Now;

                _context.Update(usuario);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Contraseña actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Profile/Activity
        public async Task<IActionResult> Activity()
        {
            // Verificar si el usuario está autenticado
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Obtener los últimos permisos, omisiones y reportes del usuario
            var permisos = await _context.Permisos
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.FechaSolicitud)
                .Take(5)
                .ToListAsync();

            var omisiones = await _context.OmisionesMarca
                .Where(o => o.UsuarioId == usuarioId)
                .OrderByDescending(o => o.FechaRegistro)
                .Take(5)
                .ToListAsync();

            var reportes = await _context.ReportesDanos
                .Where(r => r.UsuarioId == usuarioId)
                .OrderByDescending(r => r.FechaReporte)
                .Take(5)
                .ToListAsync();

            ViewBag.Permisos = permisos;
            ViewBag.Omisiones = omisiones;
            ViewBag.Reportes = reportes;

            return View();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}

