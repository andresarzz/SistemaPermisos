using SistemaPermisos.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaPermisos.Services
{
    public interface IAuditService
    {
        Task LogAsync(string action, string entity, int? entityId, string? oldValues, string? newValues);
        Task LogActivityAsync(int? userId, string action, string entity, int? entityId, string? oldValues = null, string? newValues = null);
        Task LogActivityAsync(int? userId, string action, string entity, int? entityId, string? oldValues, string? newValues, string? description);
        Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int? userId = null, string? action = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<AuditLog>> GetUserActivityAsync(int userId, int count = 10);
        Task<IEnumerable<AuditLog>> GetAllActivityAsync(int count = 100);
        Task<PaginatedList<AuditLog>> GetAllActivityAsync(int pageIndex, int pageSize);
        Task<PaginatedList<AuditLog>> GetPaginatedAuditLogsAsync(int pageIndex, int pageSize, int? userId = null, string? action = null);
    }
}
