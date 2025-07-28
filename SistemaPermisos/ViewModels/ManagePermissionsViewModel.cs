using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.ViewModels
{
    public class ManagePermissionsViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "Nombre de Usuario")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Permisos del Usuario")]
        public List<string> UserPermissions { get; set; } = new List<string>();

        [Display(Name = "Todos los Permisos Disponibles")]
        public List<string> AllPermissions { get; set; } = new List<string>();

        // Propiedad para recibir los permisos seleccionados desde el formulario
        public List<string> SelectedPermissions { get; set; } = new List<string>();
    }
}
