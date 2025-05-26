using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPermisos.Models
{
    public class TwoFactorAuth
    {
        [Key]
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        [Required]
        [StringLength(100)]
        public string ClaveSecreta { get; set; } = string.Empty;

        public bool Habilitado { get; set; } = false;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        [StringLength(10)]
        public string? UltimoCodigo { get; set; }

        public DateTime? FechaUltimoCodigo { get; set; }

        // Navegación
        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }
    }
}
