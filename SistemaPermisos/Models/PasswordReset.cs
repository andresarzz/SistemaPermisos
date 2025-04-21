using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.Models
{
    public class PasswordReset
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [Display(Name = "Token")]
        public string Token { get; set; }

        [Required]
        [Display(Name = "Fecha de Expiración")]
        public DateTime FechaExpiracion { get; set; }

        [Required]
        [Display(Name = "Utilizado")]
        public bool Utilizado { get; set; } = false;

        [Required]
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Relaciones
        public virtual Usuario Usuario { get; set; }
    }
}

