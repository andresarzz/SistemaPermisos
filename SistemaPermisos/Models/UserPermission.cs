using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPermisos.Models
{
    public class UserPermission
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(100)]
        public string Permiso { get; set; }

        // Relación con Usuario
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }
    }
}
