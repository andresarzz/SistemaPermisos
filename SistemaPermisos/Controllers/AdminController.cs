﻿using Microsoft.AspNetCore.Http;
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
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IAuditService _auditService;

        public AdminController(ApplicationDbContext context, IUserService userService, IAuditService auditService)
        {
            _context = context;
            _userService = userService;
            _auditService = auditService;
        }

        public async Task<IActionResult> Index()
        {
            return await Dashboard();
        }

        public async Task<IActionResult> Dashboard()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var usuarioRol = HttpContext.Session.GetString("UsuarioRol");

            if (usuarioId == null || usuarioRol != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var totalUsuarios = await _context.Usuarios.CountAsync();
            var usuariosActivos = await _context.Usuarios.CountAsync(u => u.Activo);
            var totalPermisos = await _context.Permisos.CountAsync();
            var permisosPendientes = await _context.Permisos.CountAsync(p => p.Estado == "Pendiente");
            var totalOmisiones = await _context.OmisionesMarca.CountAsync();
            var omisionesPendientes = await _context.OmisionesMarca.CountAsync(o => o.Estado == "Pendiente");
            var totalReportes = await _context.ReportesDanos.CountAsync();

            var actividadReciente = await _auditService.GetAllActivityAsync(1, 10);

            var viewModel = new AdminDashboardViewModel
            {
                TotalUsuarios = totalUsuarios,
                UsuariosActivos = usuariosActivos,
                TotalPermisos = totalPermisos,
                PermisosPendientes = permisosPendientes,
                TotalOmisiones = totalOmisiones,
                OmisionesPendientes = omisionesPendientes,
                TotalReportes = totalReportes,
                ActividadReciente = actividadReciente.Items.ToList()
            };

            return View("Dashboard", viewModel);
        }
    }
}
