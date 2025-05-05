using SistemaPermisos.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaPermisos.Services
{
    public interface IAuditService
    {
        Task LogActivityAsync(int? usuarioId, string accion, string entidad, int? entidadId = null, string datosAntiguos = null, string datosNuevos = null, string ip = null);
        Task<IEnumerable<AuditLog>> GetUserActivityAsync(int usuarioId, int limit = 50);
        Task<IEnumerable<AuditLog>> GetAllActivityAsync(int limit = 100);
        Task<IEnumerable<AuditLog>> GetActivityByEntityAsync(string entidad, int limit = 50);
        Task<IEnumerable<AuditLog>> GetActivityByActionAsync(string accion, int limit = 50);
    }
}
