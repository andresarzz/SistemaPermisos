using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.ViewModels
{
    public class ManagePermissionsViewModel
    {
        public int UsuarioId { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Permisos")]
        public List<string> Permisos { get; set; } = new List<string>();

        // Lista de todos los permisos disponibles en el sistema
        public static List<string> PermisosDisponibles => new List<string>
        {
            "usuarios.ver",
            "usuarios.crear",
            "usuarios.editar",
            "usuarios.eliminar",
            "permisos.ver",
            "permisos.crear",
            "permisos.editar",
            "permisos.aprobar",
            "permisos.rechazar",
            "omisiones.ver",
            "omisiones.crear",
            "omisiones.editar",
            "omisiones.aprobar",
            "omisiones.rechazar",
            "reportes.ver",
            "reportes.crear",
            "reportes.editar",
            "reportes.resolver",
            "auditoria.ver",
            "exportar.excel",
            "exportar.pdf"
        };
    }
}
