using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPermisos.Models
{
    public class ReporteDano
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;

        [Required(ErrorMessage = "El tipo de daño es obligatorio")]
        [StringLength(100)]
        public string TipoDano { get; set; } = string.Empty; // Ej. "Hardware", "Software", "Infraestructura"

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(1000)]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha del reporte es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaReporte { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "El equipo es obligatorio")]
        [StringLength(255)]
        public string Equipo { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Ubicacion { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        [StringLength(255)]
        public string? RutaEvidencia { get; set; } // Ruta al archivo de evidencia (imagen, video, etc.)

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Pendiente"; // Ej. "Pendiente", "En Proceso", "Resuelto", "Rechazado"

        [StringLength(500)]
        public string? ObservacionesResolucion { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaResolucion { get; set; }

        public int? ResueltoPorId { get; set; }

        [ForeignKey("ResueltoPorId")]
        public virtual Usuario? ResueltoPor { get; set; }
    }
}
