using SistemaPermisos.Models;
using System.Collections.Generic;

namespace SistemaPermisos.ViewModels
{
    public class DocenteDashboardViewModel
    {
        public int TotalPermisosSolicitados { get; set; }
        public int PermisosAprobados { get; set; }
        public int PermisosRechazados { get; set; }
        public int PermisosPendientes { get; set; }
        public int TotalOmisionesSolicitadas { get; set; }
        public int OmisionesAprobadas { get; set; }
        public int OmisionesRechazadas { get; set; }
        public int OmisionesPendientes { get; set; }
        public int TotalReportesCreados { get; set; }
        public int ReportesResueltos { get; set; }
        public int ReportesEnProceso { get; set; }
        public int ReportesPendientes { get; set; }
        public List<Permiso> MisPermisosRecientes { get; set; } = new List<Permiso>();
        public List<OmisionMarca> MisOmisionesRecientes { get; set; } = new List<OmisionMarca>();
        public List<ReporteDano> MisReportesRecientes { get; set; } = new List<ReporteDano>();
        public List<AuditLog> ActividadReciente { get; set; } = new List<AuditLog>();
    }
}
