using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Models;
using SistemaPermisos.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaPermisos.Services
{
    public class AuditService : IAuditService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogActivityAsync(int? usuarioId, string accion, string tabla, int? registroId = null, string datosAntiguos = null, string datosNuevos = null, string ip = null)
        {
            if (string.IsNullOrEmpty(ip))
            {
                ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Desconocida";
            }

            var auditLog = new AuditLog
            {
                UsuarioId = usuarioId,
                Accion = accion,
                Tabla = tabla,
                RegistroId = registroId,
                DatosAntiguos = datosAntiguos,
                DatosNuevos = datosNuevos,
                DireccionIP = ip,
                Fecha = DateTime.Now
            };

            await _unitOfWork.AuditLogs.AddAsync(auditLog);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetUserActivityAsync(int usuarioId, int limit = 50)
        {
            return await _unitOfWork.AuditLogs.Query()
                .Where(a => a.UsuarioId == usuarioId)
                .OrderByDescending(a => a.Fecha)
                .Take(limit)
                .Include(a => a.Usuario)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAllActivityAsync(int limit = 100)
        {
            return await _unitOfWork.AuditLogs.Query()
                .OrderByDescending(a => a.Fecha)
                .Take(limit)
                .Include(a => a.Usuario)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetActivityByTableAsync(string tabla, int limit = 50)
        {
            return await _unitOfWork.AuditLogs.Query()
                .Where(a => a.Tabla == tabla)
                .OrderByDescending(a => a.Fecha)
                .Take(limit)
                .Include(a => a.Usuario)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetActivityByActionAsync(string accion, int limit = 50)
        {
            return await _unitOfWork.AuditLogs.Query()
                .Where(a => a.Accion == accion)
                .OrderByDescending(a => a.Fecha)
                .Take(limit)
                .Include(a => a.Usuario)
                .ToListAsync();
        }
    }
}

