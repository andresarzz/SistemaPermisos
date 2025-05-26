using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPermisos.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        public int? UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Acción")]
        public string Accion { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Entidad")]
        public string Entidad { get; set; }

        [Display(Name = "ID de Entidad")]
        public int? EntidadId { get; set; }

        [Display(Name = "Datos Antiguos")]
        public string DatosAntiguos { get; set; }

        [Display(Name = "Datos Nuevos")]
        public string DatosNuevos { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Dirección IP")]
        public string DireccionIP { get; set; }

        [Display(Name = "User Agent")]
        public string UserAgent { get; set; }

        [Required]
        [Display(Name = "Fecha y Hora")]
        public DateTime FechaHora { get; set; } = DateTime.Now;

        // Propiedades de compatibilidad para código existente
        [NotMapped]
        public string Tabla
        {
            get { return Entidad; }
            set { Entidad = value; }
        }

        [NotMapped]
        public DateTime Fecha
        {
            get { return FechaHora; }
            set { FechaHora = value; }
        }

        [NotMapped]
        public int? RegistroId
        {
            get { return EntidadId; }
            set { EntidadId = value; }
        }
    }
}
