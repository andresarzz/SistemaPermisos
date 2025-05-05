using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        [Display(Name = "Usuario")]
        public int? UsuarioId { get; set; }

        [Required]
        [Display(Name = "Acción")]
        public string Accion { get; set; }

        [Required]
        [Display(Name = "Entidad")]
        public string Entidad { get; set; }

        [Display(Name = "ID de Entidad")]
        public int? EntidadId { get; set; }

        [Display(Name = "Detalles")]
        public string Detalles { get; set; }

        [Required]
        [Display(Name = "Dirección IP")]
        public string DireccionIP { get; set; }

        [Display(Name = "User Agent")]
        public string UserAgent { get; set; }

        [Required]
        [Display(Name = "Fecha y Hora")]
        public DateTime FechaHora { get; set; } = DateTime.Now;

        // Relaciones
        public virtual Usuario Usuario { get; set; }
    }
}
