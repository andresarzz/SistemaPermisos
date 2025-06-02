using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaPermisos.Models;
using SistemaPermisos.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SistemaPermisos.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAuditService _auditService;

        public HomeController(ILogger<HomeController> logger, IAuditService auditService)
        {
            _logger = logger;
            _auditService = auditService;
        }

        public async Task<IActionResult> Index()
        {
            // Verificar si el usuario está autenticado
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var usuarioRol = HttpContext.Session.GetString("UsuarioRol");
            var usuarioNombre = HttpContext.Session.GetString("UsuarioNombre");

            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Redirigir según el rol del usuario
            switch (usuarioRol)
            {
                case "Admin":
                    return RedirectToAction("Dashboard", "Admin");
                case "Supervisor":
                    return RedirectToAction("Dashboard", "Supervisor");
                case "Docente":
                default:
                    return RedirectToAction("Dashboard", "Docente");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
