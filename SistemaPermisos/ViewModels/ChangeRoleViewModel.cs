using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.ViewModels
{
    public class ChangeRoleViewModel
    {
        public int UsuarioId { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Rol Actual")]
        public string RolActual { get; set; }

        [Required(ErrorMessage = "El nuevo rol es obligatorio")]
        [Display(Name = "Nuevo Rol")]
        public string NuevoRol { get; set; }
    }
}

