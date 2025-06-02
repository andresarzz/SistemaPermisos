using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Data;
using SistemaPermisos.Models;
using SistemaPermisos.Services;
using SistemaPermisos.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaPermisos.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IAuditService _auditService;
        private readonly IEmailService _emailService;

        public AccountController(
            ApplicationDbContext context,
            IUserService userService,
            IAuditService auditService,
            IEmailService emailService)
        {
            _context = context;
            _userService = userService;
            _auditService = auditService;
            _emailService = emailService;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            // Si ya está autenticado, redirigir al inicio
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == model.Correo);

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

                        // Registrar en auditoría
                        try
                        {
                            await _auditService.LogActivityAsync(
                                usuario.Id,
                                "Iniciar Sesión",
                                "Usuario",
                                usuario.Id,
                                null,
                                null,
                                "Inicio de sesión exitoso"
                            );
                        }
                        catch (Exception auditEx)
                        {
                            // Log audit error but don't fail login
                            System.Diagnostics.Debug.WriteLine($"Error en auditoría: {auditEx.Message}");
                        }

                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError("", "Correo o contraseña incorrectos");
                }
                catch (Exception ex)
                {
                    // Registrar la excepción
                    System.Diagnostics.Debug.WriteLine($"Error al iniciar sesión: {ex.Message}");
                    ModelState.AddModelError("", "Ocurrió un error al procesar la solicitud. Intente nuevamente.");
                }
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
                try
                {
                    // Verificar si el correo ya existe
                    if (await _context.Usuarios.AnyAsync(u => u.Correo == model.Correo))
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

                    // Registrar en auditoría
                    try
                    {
                        await _auditService.LogActivityAsync(
                            usuarioId,
                            "Crear",
                            "Usuario",
                            usuario.Id,
                            null,
                            null,
                            $"Usuario creado: {usuario.Nombre} ({usuario.Correo})"
                        );
                    }
                    catch (Exception auditEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error en auditoría: {auditEx.Message}");
                    }

                    // Enviar correo de bienvenida
                    try
                    {
                        await _emailService.SendWelcomeEmailAsync(usuario.Correo, usuario.Nombre);
                    }
                    catch (Exception emailEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error enviando email: {emailEx.Message}");
                    }

                    TempData["SuccessMessage"] = "Usuario registrado correctamente.";
                    return RedirectToAction("Index", "Users");
                }
                catch (Exception ex)
                {
                    // Registrar la excepción
                    System.Diagnostics.Debug.WriteLine($"Error al registrar usuario: {ex.Message}");
                    ModelState.AddModelError("", "Ocurrió un error al procesar la solicitud. Intente nuevamente.");
                }
            }

            return View(model);
        }

        // GET: Account/Logout
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Obtener el ID del usuario antes de limpiar la sesión
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

                if (usuarioId.HasValue)
                {
                    // Registrar en auditoría
                    try
                    {
                        await _auditService.LogActivityAsync(
                            usuarioId.Value,
                            "Cerrar Sesión",
                            "Usuario",
                            usuarioId.Value,
                            null,
                            null,
                            "Cierre de sesión"
                        );
                    }
                    catch (Exception auditEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error en auditoría: {auditEx.Message}");
                    }
                }

                // Limpiar la sesión
                HttpContext.Session.Clear();
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al cerrar sesión: {ex.Message}");
            }

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
                try
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

                    // Guardar token en la base de datos
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

                    // Registrar en auditoría
                    try
                    {
                        await _auditService.LogActivityAsync(
                            null,
                            "Solicitar Restablecimiento",
                            "Usuario",
                            usuario.Id,
                            null,
                            null,
                            "Solicitud de restablecimiento de contraseña"
                        );
                    }
                    catch (Exception auditEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error en auditoría: {auditEx.Message}");
                    }

                    // Construir el enlace de restablecimiento
                    var resetLink = Url.Action("ResetPassword", "Account", new { token }, Request.Scheme);

                    // Enviar correo con el enlace
                    try
                    {
                        await _emailService.SendPasswordResetEmailAsync(usuario.Correo, resetLink);
                    }
                    catch (Exception emailEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error enviando email: {emailEx.Message}");
                    }

                    TempData["SuccessMessage"] = "Se han enviado instrucciones a su correo electrónico.";
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    // Registrar la excepción
                    System.Diagnostics.Debug.WriteLine($"Error al procesar solicitud de restablecimiento: {ex.Message}");
                    ModelState.AddModelError("", "Ocurrió un error al procesar la solicitud. Intente nuevamente.");
                }
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

            try
            {
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
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al verificar token: {ex.Message}");
                TempData["ErrorMessage"] = "Ocurrió un error al procesar la solicitud. Intente nuevamente.";
                return RedirectToAction("Login");
            }
        }

        // POST: Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
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

                    // Registrar en auditoría
                    try
                    {
                        await _auditService.LogActivityAsync(
                            usuario.Id,
                            "Restablecer Contraseña",
                            "Usuario",
                            usuario.Id,
                            null,
                            null,
                            "Contraseña restablecida correctamente"
                        );
                    }
                    catch (Exception auditEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error en auditoría: {auditEx.Message}");
                    }

                    TempData["SuccessMessage"] = "Su contraseña ha sido restablecida correctamente.";
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    // Registrar la excepción
                    System.Diagnostics.Debug.WriteLine($"Error al restablecer contraseña: {ex.Message}");
                    ModelState.AddModelError("", "Ocurrió un error al procesar la solicitud. Intente nuevamente.");
                }
            }

            return View(model);
        }

        // GET: Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
