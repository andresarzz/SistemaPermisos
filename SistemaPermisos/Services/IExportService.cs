using SistemaPermisos.Models;
using System.Collections.Generic;

namespace SistemaPermisos.Services
{
    public interface IExportService
    {
        byte[] ExportUsersToExcel(List<Usuario> users);
        byte[] ExportPermisosToExcel(List<Permiso> permisos);
        byte[] ExportOmisionesToExcel(List<OmisionMarca> omisiones);
        byte[] ExportReportesToExcel(List<ReporteDano> reportes);
        byte[] ExportAuditLogsToExcel(List<AuditLog> auditLogs);
    }
}
