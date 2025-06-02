using SistemaPermisos.Models;
using System.Collections.Generic;

namespace SistemaPermisos.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsuarios { get; set; }
        public int UsuariosActivos { get; set; }
        public int TotalPermisos { get; set; }
        public int PermisosPendientes { get; set; }
        public int TotalOmisiones { get; set; }
        public int OmisionesPendientes { get; set; }
        public int TotalReportes { get; set; }
        public List<AuditLog> ActividadReciente { get; set; } = new List<AuditLog>();
    }
}
