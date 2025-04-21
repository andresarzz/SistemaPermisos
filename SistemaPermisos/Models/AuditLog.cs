using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        [Required]
        public int? UsuarioId { get; set; }

        [Required]
        [Display(Name = "Acción")]
        public string Accion { get; set; }

        [Required]
        [Display(Name = "Tabla")]
        public string Tabla { get; set; }

        [Display(Name = "Registro ID")]
        public int? RegistroId { get; set; }

        [Display(Name = "Datos Antiguos")]
        public string DatosAntiguos { get; set; }

        [Display(Name = "Datos Nuevos")]
        public string DatosNuevos { get; set; }

        [Required]
        [Display(Name = "Dirección IP")]
        public string DireccionIP { get; set; }

        [Required]
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        // Relaciones
        public virtual Usuario Usuario { get; set; }
    }
}

