using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Data;
using SistemaPermisos.Models;
using SistemaPermisos.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaPermisos.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AccountController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            // Si ya está autenticado, redirigir al inicio
            if (HttpContext.Session.GetInt32("UsuarioId") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == model.Correo);

                if (usuario != null && BCrypt.Net.BCrypt.Verify(model.Password, usuario.Password))
                {
                    // Verificar si el usuario está activo
                    if (!usuario.Activo)
                    {
                        ModelState.AddModelError("", "Esta cuenta ha sido desactivada. Contacte al administrador.");
                        return View(model);
                    }

                    // Guardar información del usuario en la sesión
                    HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
                    HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
                    HttpContext.Session.SetString("UsuarioRol", usuario.Rol);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Correo o contraseña incorrectos");
            }

            return View(model);
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            // Verificar si el usuario está autenticado y es administrador
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var rol = HttpContext.Session.GetString("UsuarioRol");

            if (usuarioId == null || rol != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Verificar si el usuario está autenticado y es administrador
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var rol = HttpContext.Session.GetString("UsuarioRol");

            if (usuarioId == null || rol != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                // Verificar si el correo ya existe
                if (_context.Usuarios.Any(u => u.Correo == model.Correo))
                {
                    ModelState.AddModelError("Correo", "Este correo ya está registrado");
                    return View(model);
                }

                // Crear nuevo usuario
                var usuario = new Usuario
                {
                    Nombre = model.Nombre,
                    Correo = model.Correo,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Rol = "Docente", // Por defecto es Docente
                    FechaRegistro = DateTime.Now,
                    UltimaActualizacion = DateTime.Now,
                    Activo = true
                };

                _context.Add(usuario);
                await _context.SaveChangesAsync();

                // Registrar en auditoría si existe el servicio
                // Aquí se podría agregar código para registrar la acción en la auditoría

                TempData["SuccessMessage"] = "Usuario registrado correctamente.";
                return RedirectToAction("Index", "Users");
            }

            return View(model);
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            // Limpiar la sesión
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }

        // GET: Account/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == model.Correo);
                if (usuario == null)
                {
                    // No revelar que el usuario no existe
                    TempData["SuccessMessage"] = "Si su correo está registrado, recibirá instrucciones para restablecer su contraseña.";
                    return RedirectToAction("Login");
                }

                // Generar token de restablecimiento
                string token = Guid.NewGuid().ToString();
                DateTime expiracion = DateTime.Now.AddHours(24);

                // Guardar token en la base de datos (si existe la tabla PasswordResets)
                var passwordReset = new PasswordReset
                {
                    UsuarioId = usuario.Id,
                    Token = token,
                    FechaExpiracion = expiracion,
                    Utilizado = false,
                    FechaCreacion = DateTime.Now
                };

                _context.Add(passwordReset);
                await _context.SaveChangesAsync();

                // Aquí se enviaría un correo con el enlace para restablecer la contraseña
                // Por ahora, solo mostramos un mensaje de éxito
                TempData["SuccessMessage"] = "Se han enviado instrucciones a su correo electrónico.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        // GET: Account/ResetPassword
        public async Task<IActionResult> ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            // Verificar si el token es válido
            var passwordReset = await _context.PasswordResets
                .FirstOrDefaultAsync(pr => pr.Token == token && !pr.Utilizado && pr.FechaExpiracion > DateTime.Now);

            if (passwordReset == null)
            {
                TempData["ErrorMessage"] = "El enlace para restablecer la contraseña es inválido o ha expirado.";
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordViewModel
            {
                Token = token
            };

            return View(model);
        }

        // POST: Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el token es válido
                var passwordReset = await _context.PasswordResets
                    .FirstOrDefaultAsync(pr => pr.Token == model.Token && !pr.Utilizado && pr.FechaExpiracion > DateTime.Now);

                if (passwordReset == null)
                {
                    TempData["ErrorMessage"] = "El enlace para restablecer la contraseña es inválido o ha expirado.";
                    return RedirectToAction("Login");
                }

                // Actualizar contraseña del usuario
                var usuario = await _context.Usuarios.FindAsync(passwordReset.UsuarioId);
                if (usuario == null)
                {
                    TempData["ErrorMessage"] = "Usuario no encontrado.";
                    return RedirectToAction("Login");
                }

                usuario.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                usuario.UltimaActualizacion = DateTime.Now;

                // Marcar el token como utilizado
                passwordReset.Utilizado = true;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Su contraseña ha sido restablecida correctamente.";
                return RedirectToAction("Login");
            }

            return View(model);
        }
    }
}
