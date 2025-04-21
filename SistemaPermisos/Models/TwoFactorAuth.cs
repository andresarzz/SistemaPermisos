using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.Models
{
    public class TwoFactorAuth
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [Display(Name = "Habilitado")]
        public bool Habilitado { get; set; } = false;

        [Display(Name = "Clave Secreta")]
        public string ClaveSecreta { get; set; }

        [Display(Name = "Último Código")]
        public string UltimoCodigo { get; set; }

        [Display(Name = "Fecha de Expiración del Código")]
        public DateTime? FechaExpiracionCodigo { get; set; }

        [Display(Name = "Fecha de Actualización")]
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        // Relaciones
        public virtual Usuario Usuario { get; set; }
    }
}

