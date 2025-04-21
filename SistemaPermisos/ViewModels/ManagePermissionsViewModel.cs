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
    }
}
