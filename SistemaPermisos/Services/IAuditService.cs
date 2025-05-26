using SistemaPermisos.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaPermisos.Services
{
    public interface IAuditService
    {
        Task<bool> LogActivityAsync(int? userId, string action, string entity, int? entityId, string oldData = null, string newData = null);
        Task<IEnumerable<AuditLog>> GetUserActivityAsync(int userId, int count = 10);
        Task<IEnumerable<AuditLog>> GetEntityActivityAsync(string entity, int entityId, int count = 10);
        Task<IEnumerable<AuditLog>> GetAllActivityAsync(int count = 100);
        Task<IEnumerable<AuditLog>> SearchActivityAsync(string searchTerm, int count = 100);
    }
}
