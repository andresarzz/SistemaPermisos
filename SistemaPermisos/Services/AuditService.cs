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

        public async Task<bool> LogActivityAsync(int? userId, string action, string entity, int? entityId, string oldData = null, string newData = null)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
                var userAgent = httpContext?.Request?.Headers["User-Agent"].ToString() ?? "Unknown";

                // Si no se proporciona userId, intentar obtenerlo de la sesión
                if (!userId.HasValue && httpContext?.Session != null)
                {
                    var userIdString = httpContext.Session.GetString("UsuarioId");
                    if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int id))
                    {
                        userId = id;
                    }
                }

                var auditLog = new AuditLog
                {
                    UsuarioId = userId,
                    Accion = action,
                    Tabla = entity,
                    EntidadId = entityId,
                    DatosAntiguos = oldData,
                    DatosNuevos = newData,
                    DireccionIP = ipAddress,
                    UserAgent = userAgent,
                    Fecha = DateTime.Now
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al registrar actividad: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<AuditLog>> GetUserActivityAsync(int userId, int count = 10)
        {
            try
            {
                return await _context.AuditLogs
                    .Where(a => a.UsuarioId == userId)
                    .OrderByDescending(a => a.Fecha)
                    .Take(count)
                    .Include(a => a.Usuario)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al obtener actividad del usuario: {ex.Message}");
                return Enumerable.Empty<AuditLog>();
            }
        }

        public async Task<IEnumerable<AuditLog>> GetEntityActivityAsync(string entity, int entityId, int count = 10)
        {
            try
            {
                return await _context.AuditLogs
                    .Where(a => a.Tabla == entity && a.EntidadId == entityId)
                    .OrderByDescending(a => a.Fecha)
                    .Take(count)
                    .Include(a => a.Usuario)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al obtener actividad de la entidad: {ex.Message}");
                return Enumerable.Empty<AuditLog>();
            }
        }

        public async Task<IEnumerable<AuditLog>> GetAllActivityAsync(int count = 100)
        {
            try
            {
                return await _context.AuditLogs
                    .OrderByDescending(a => a.Fecha)
                    .Take(count)
                    .Include(a => a.Usuario)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al obtener toda la actividad: {ex.Message}");
                return Enumerable.Empty<AuditLog>();
            }
        }

        public async Task<IEnumerable<AuditLog>> SearchActivityAsync(string searchTerm, int count = 100)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return await GetAllActivityAsync(count);
                }

                return await _context.AuditLogs
                    .Where(a =>
                        a.Accion.Contains(searchTerm) ||
                        a.Tabla.Contains(searchTerm) ||
                        (a.DatosAntiguos != null && a.DatosAntiguos.Contains(searchTerm)) ||
                        (a.DatosNuevos != null && a.DatosNuevos.Contains(searchTerm)) ||
                        a.DireccionIP.Contains(searchTerm) ||
                        (a.Usuario != null && a.Usuario.Nombre.Contains(searchTerm)))
                    .OrderByDescending(a => a.Fecha)
                    .Take(count)
                    .Include(a => a.Usuario)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al buscar actividad: {ex.Message}");
                return Enumerable.Empty<AuditLog>();
            }
        }
    }
}
