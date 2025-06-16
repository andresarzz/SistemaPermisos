using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPermisos.Models
{
    public class TwoFactorAuth
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(10)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Tipo { get; set; } = string.Empty; // SMS, Email

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime FechaExpiracion { get; set; }

        public bool Usado { get; set; } = false;

        // Navegación
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;
    }
}
