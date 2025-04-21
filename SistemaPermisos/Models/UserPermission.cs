using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.Models
{
    public class UserPermission
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [Display(Name = "Permiso")]
        public string Permiso { get; set; }

        // Relaciones
        public virtual Usuario Usuario { get; set; }
    }
}

