using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SistemaPermisos.Data;
using SistemaPermisos.Models;
using SistemaPermisos.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaPermisos.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        // Modificar el método Login para verificar si el usuario está activo
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



        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
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
                    Rol = "Docente" // Por defecto todos son docentes
                };

                _context.Add(usuario);
                await _context.SaveChangesAsync();

                // Iniciar sesión automáticamente
                HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
                HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
                HttpContext.Session.SetString("UsuarioRol", usuario.Rol);

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}

