using SistemaPermisos.Models;
using System.Collections.Generic;

namespace SistemaPermisos.ViewModels
{
    public class SupervisorDashboardViewModel
    {
        public int TotalPermisosPendientes { get; set; }
        public int TotalOmisionesPendientes { get; set; }
        public int TotalReportesPendientes { get; set; }
        public List<Permiso> PermisosPendientes { get; set; } = new List<Permiso>();
        public List<OmisionMarca> OmisionesPendientes { get; set; } = new List<OmisionMarca>();
        public List<ReporteDano> ReportesPendientes { get; set; } = new List<ReporteDano>();
        public List<AuditLog> ActividadReciente { get; set; } = new List<AuditLog>();
    }
}
