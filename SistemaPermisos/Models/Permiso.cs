using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPermisos.Models
{
    public class Permiso
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;

        [Required(ErrorMessage = "El tipo de permiso es obligatorio")]
        [StringLength(50)]
        public string TipoPermiso { get; set; } = string.Empty; // Ej. "Vacaciones", "Enfermedad", "Asuntos Personales"

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? HoraDesde { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan? HoraHasta { get; set; }

        public bool JornadaCompleta { get; set; } = false;
        public bool MediaJornada { get; set; } = false;
        public int? CantidadLecciones { get; set; }

        [StringLength(20)]
        public string? Cedula { get; set; }

        [StringLength(100)]
        public string? Puesto { get; set; }

        [StringLength(50)]
        public string? Condicion { get; set; }

        [StringLength(100)]
        public string? TipoMotivo { get; set; }

        [StringLength(100)]
        public string? TipoConvocatoria { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio")]
        [StringLength(500)]
        public string Motivo { get; set; } = string.Empty;

        [StringLength(255)]
        public string? DocumentoAdjunto { get; set; } // Ruta al archivo adjunto

        [StringLength(500)]
        public string? Justificacion { get; set; } // Campo para la justificación de un permiso

        [DataType(DataType.Date)]
        public DateTime? FechaJustificacion { get; set; } // Fecha de la justificación

        [DataType(DataType.Date)]
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Pendiente"; // Ej. "Pendiente", "Aprobado", "Rechazado", "Justificado"

        [StringLength(500)]
        public string? ObservacionesResolucion { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaResolucion { get; set; }

        public int? AprobadoPorId { get; set; }

        [ForeignKey("AprobadoPorId")]
        public virtual Usuario? AprobadoPor { get; set; }

        [StringLength(1000)]
        public string? ComentariosAprobador { get; set; }

        [StringLength(50)]
        public string? TipoRebajo { get; set; } // Por ejemplo: "Vacaciones", "Días Libres", "Sin Goce de Salario"
    }
}
