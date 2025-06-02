using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Data;
using SistemaPermisos.Services;
using SistemaPermisos.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaPermisos.Controllers
{
    public class DocenteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public DocenteController(ApplicationDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<IActionResult> Dashboard()
        {
            // Verificar autenticación
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var usuarioRol = HttpContext.Session.GetString("UsuarioRol");

            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Obtener estadísticas del docente
            var misPermisos = await _context.Permisos.CountAsync(p => p.UsuarioId == usuarioId);
            var permisosPendientes = await _context.Permisos.CountAsync(p => p.UsuarioId == usuarioId && p.Estado == "Pendiente");
            var permisosAprobados = await _context.Permisos.CountAsync(p => p.UsuarioId == usuarioId && p.Estado == "Aprobado");
            var misOmisiones = await _context.OmisionesMarca.CountAsync(o => o.UsuarioId == usuarioId);

            var viewModel = new DocenteDashboardViewModel
            {
                MisPermisos = misPermisos,
                PermisosPendientes = permisosPendientes,
                PermisosAprobados = permisosAprobados,
                MisOmisiones = misOmisiones
            };

            return View(viewModel);
        }

        public async Task<IActionResult> MisPermisos()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var permisos = await _context.Permisos
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.FechaSolicitud)
                .ToListAsync();

            return View(permisos);
        }

        public async Task<IActionResult> MisOmisiones()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var omisiones = await _context.OmisionesMarca
                .Where(o => o.UsuarioId == usuarioId)
                .OrderByDescending(o => o.FechaOmision)
                .ToListAsync();

            return View(omisiones);
        }

        public IActionResult SolicitarPermiso()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("Create", "Permisos");
        }

        public IActionResult ReportarOmision()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("Create", "Omisiones");
        }
    }
}
