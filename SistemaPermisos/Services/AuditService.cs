using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Data;
using SistemaPermisos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaPermisos.Services
{
    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task LogActivityAsync(int? usuarioId, string accion, string entidad, int? entidadId = null, string datosAntiguos = null, string datosNuevos = null, string ip = null)
        {
            try
            {
                if (string.IsNullOrEmpty(ip))
                {
                    ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Desconocida";
                }

                var auditLog = new AuditLog
                {
                    UsuarioId = usuarioId,
                    Accion = accion,
                    Entidad = entidad,
                    EntidadId = entidadId,
                    Detalles = string.IsNullOrEmpty(datosNuevos) ? datosAntiguos : datosNuevos,
                    DireccionIP = ip,
                    UserAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString(),
                    FechaHora = DateTime.Now
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Registrar la excepción en algún lugar (por ejemplo, en un archivo de registro)
                System.Diagnostics.Debug.WriteLine($"Error al registrar actividad: {ex.Message}");
                // No lanzar la excepción para evitar interrumpir el flujo principal
            }
        }

        public async Task<IEnumerable<AuditLog>> GetUserActivityAsync(int usuarioId, int limit = 50)
        {
            return await _context.AuditLogs
                .Where(a => a.UsuarioId == usuarioId)
                .OrderByDescending(a => a.FechaHora)
                .Take(limit)
                .Include(a => a.Usuario)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAllActivityAsync(int limit = 100)
        {
            return await _context.AuditLogs
                .OrderByDescending(a => a.FechaHora)
                .Take(limit)
                .Include(a => a.Usuario)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetActivityByEntityAsync(string entidad, int limit = 50)
        {
            return await _context.AuditLogs
                .Where(a => a.Entidad == entidad)
                .OrderByDescending(a => a.FechaHora)
                .Take(limit)
                .Include(a => a.Usuario)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetActivityByActionAsync(string accion, int limit = 50)
        {
            return await _context.AuditLogs
                .Where(a => a.Accion == accion)
                .OrderByDescending(a => a.FechaHora)
                .Take(limit)
                .Include(a => a.Usuario)
                .ToListAsync();
        }
    }
}
