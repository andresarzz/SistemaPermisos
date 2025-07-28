using Microsoft.AspNetCore.Http;
using SistemaPermisos.Data;
using SistemaPermisos.Models;
using System;
using System.Threading.Tasks;

namespace SistemaPermisos.Services
{
    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAction(int userId, string action, string? details = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();
            var userAgent = httpContext?.Request.Headers["User-Agent"].ToString();

            var auditLog = new AuditLog
            {
                UsuarioId = userId,
                Accion = action,
                Detalles = details,
                FechaHora = DateTime.Now,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
    }
}
