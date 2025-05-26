using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPermisos.Models
{
    public class PasswordReset
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(100)]
        public string Token { get; set; }

        [Required]
        public DateTime FechaExpiracion { get; set; }

        [Required]
        public bool Utilizado { get; set; } = false;

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Relación con Usuario
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }
    }
}
