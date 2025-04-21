using SistemaPermisos.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaPermisos.Services
{
    public interface IExportService
    {
        Task<byte[]> ExportUsersToExcelAsync(IEnumerable<Usuario> users);
        Task<byte[]> ExportUsersToPdfAsync(IEnumerable<Usuario> users);
        Task<byte[]> ExportPermisosToExcelAsync(IEnumerable<Permiso> permisos);
        Task<byte[]> ExportPermisosToPdfAsync(IEnumerable<Permiso> permisos);
        Task<byte[]> ExportOmisionesToExcelAsync(IEnumerable<OmisionMarca> omisiones);
        Task<byte[]> ExportOmisionesToPdfAsync(IEnumerable<OmisionMarca> omisiones);
        Task<byte[]> ExportAuditLogToExcelAsync(IEnumerable<AuditLog> auditLogs);
        Task<byte[]> ExportAuditLogToPdfAsync(IEnumerable<AuditLog> auditLogs);
    }
}

