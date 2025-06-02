using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Data;
using SistemaPermisos.Services;
using SistemaPermisos.ViewModels;
using System.Threading.Tasks;

namespace SistemaPermisos.Controllers
{
    public class SupervisorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public SupervisorController(ApplicationDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<IActionResult> Dashboard()
        {
            // Verificar autenticación y rol
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var usuarioRol = HttpContext.Session.GetString("UsuarioRol");

            if (usuarioId == null || (usuarioRol != "Supervisor" && usuarioRol != "Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            // Obtener estadísticas para el dashboard del supervisor
            var permisosPendientes = await _context.Permisos.CountAsync(p => p.Estado == "Pendiente");
            var permisosAprobados = await _context.Permisos.CountAsync(p => p.Estado == "Aprobado");
            var permisosRechazados = await _context.Permisos.CountAsync(p => p.Estado == "Rechazado");
            var omisionesPendientes = await _context.OmisionesMarca.CountAsync(o => o.Estado == "Pendiente");

            var viewModel = new SupervisorDashboardViewModel
            {
                PermisosPendientes = permisosPendientes,
                PermisosAprobados = permisosAprobados,
                PermisosRechazados = permisosRechazados,
                OmisionesPendientes = omisionesPendientes
            };

            return View(viewModel);
        }

        public IActionResult Permisos()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var usuarioRol = HttpContext.Session.GetString("UsuarioRol");

            if (usuarioId == null || (usuarioRol != "Supervisor" && usuarioRol != "Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("Index", "Permisos");
        }

        public IActionResult Omisiones()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var usuarioRol = HttpContext.Session.GetString("UsuarioRol");

            if (usuarioId == null || (usuarioRol != "Supervisor" && usuarioRol != "Admin"))
            {
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("Index", "Omisiones");
        }
    }
}
