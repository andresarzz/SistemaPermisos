using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPermisos.Models
{
    public class UserPermission
    {
        [Key]
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public int PermisoId { get; set; }

        [Required]
        [StringLength(100)]
        public string Permiso { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public DateTime FechaAsignacion { get; set; } = DateTime.Now;

        public DateTime? FechaRevocacion { get; set; }

        // Navegación
        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }
    }
}
