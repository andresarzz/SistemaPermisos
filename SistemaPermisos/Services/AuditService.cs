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
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string action, string entity, int? entityId, string? oldValues, string? newValues)
        {
            try
            {
                var userId = GetCurrentUserId();
                var ipAddress = GetClientIpAddress();

                var auditLog = new AuditLog
                {
                    UsuarioId = userId,
                    Accion = action,
                    Entidad = entity,
                    RegistroId = entityId,
                    ValoresAnteriores = oldValues,
                    ValoresNuevos = newValues,
                    DireccionIP = ipAddress,
                    Fecha = DateTime.Now
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error but don't throw to avoid breaking the main operation
                System.Diagnostics.Debug.WriteLine($"Error logging audit: {ex.Message}");
            }
        }

        public async Task LogActivityAsync(int? userId, string action, string entity, int? entityId, string? oldValues = null, string? newValues = null)
        {
            try
            {
                var ipAddress = GetClientIpAddress();

                var auditLog = new AuditLog
                {
                    UsuarioId = userId,
                    Accion = action,
                    Entidad = entity,
                    RegistroId = entityId,
                    ValoresAnteriores = oldValues,
                    ValoresNuevos = newValues,
                    DireccionIP = ipAddress,
                    Fecha = DateTime.Now
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error but don't throw to avoid breaking the main operation
                System.Diagnostics.Debug.WriteLine($"Error logging activity: {ex.Message}");
            }
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int? userId = null, string? action = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.AuditLogs
                .Include(al => al.Usuario)
                .AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(al => al.UsuarioId == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(action))
            {
                query = query.Where(al => al.Accion.Contains(action));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(al => al.Fecha >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(al => al.Fecha <= toDate.Value);
            }

            return await query
                .OrderByDescending(al => al.Fecha)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetUserActivityAsync(int userId, int count = 10)
        {
            return await _context.AuditLogs
                .Include(al => al.Usuario)
                .Where(al => al.UsuarioId == userId)
                .OrderByDescending(al => al.Fecha)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAllActivityAsync(int count = 100)
        {
            return await _context.AuditLogs
                .Include(al => al.Usuario)
                .OrderByDescending(al => al.Fecha)
                .Take(count)
                .ToListAsync();
        }

        public async Task<PaginatedList<AuditLog>> GetPaginatedAuditLogsAsync(int pageIndex, int pageSize, int? userId = null, string? action = null)
        {
            var query = _context.AuditLogs
                .Include(al => al.Usuario)
                .AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(al => al.UsuarioId == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(action))
            {
                query = query.Where(al => al.Accion.Contains(action));
            }

            query = query.OrderByDescending(al => al.Fecha);

            return await PaginatedList<AuditLog>.CreateAsync(query, pageIndex, pageSize);
        }

        private int? GetCurrentUserId()
        {
            try
            {
                var userIdString = _httpContextAccessor.HttpContext?.Session.GetString("UsuarioId");
                if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int userId))
                {
                    return userId;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting current user ID: {ex.Message}");
            }

            return null;
        }

        private string GetClientIpAddress()
        {
            try
            {
                var context = _httpContextAccessor.HttpContext;
                if (context == null) return "Unknown";

                var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = context.Request.Headers["X-Real-IP"].FirstOrDefault();
                }
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = context.Connection.RemoteIpAddress?.ToString();
                }

                return ipAddress ?? "Unknown";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting client IP: {ex.Message}");
                return "Unknown";
            }
        }
    }
}
