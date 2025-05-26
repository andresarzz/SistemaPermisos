using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPermisos.Models
{
    public class TwoFactorAuth
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public bool Habilitado { get; set; } = false;

        [Required]
        [StringLength(100)]
        public string ClaveSecreta { get; set; }

        [StringLength(10)]
        public string UltimoCodigo { get; set; }

        public DateTime? FechaExpiracionCodigo { get; set; }

        [Required]
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        // Relación con Usuario
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }
    }
}
