using System.Collections.Generic;
using SistemaPermisos.Models;

namespace SistemaPermisos.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsuarios { get; set; }
        public int PermisosPendientes { get; set; }
        public int PermisosAprobados { get; set; }
        public int PermisosRechazados { get; set; }
        public int OmisionesPendientes { get; set; }
        public int OmisionesAprobadas { get; set; }
        public int OmisionesRechazadas { get; set; }
        public int ReportesPendientes { get; set; }
        public int ReportesResueltos { get; set; }
        public int ReportesEnProceso { get; set; }
        public List<AuditLog> ActividadReciente { get; set; } = new List<AuditLog>();
    }
}
