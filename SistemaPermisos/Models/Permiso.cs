using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.Models
{
    public class Permiso
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El tipo de permiso es obligatorio")]
        [StringLength(50)]
        public string TipoPermiso { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        public DateTime FechaFin { get; set; }

        public TimeSpan? HoraDesde { get; set; }
        public TimeSpan? HoraHasta { get; set; }
        public bool JornadaCompleta { get; set; }
        public bool MediaJornada { get; set; }
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

        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente";

        [StringLength(500)]
        public string? Observaciones { get; set; }

        public string? HoraSalida { get; set; }

        [StringLength(255)]
        public string? RutaComprobante { get; set; }

        [StringLength(255)]
        public string? RutaJustificacion { get; set; }

        public bool Justificado { get; set; } = false;

        [StringLength(100)]
        public string? Resolucion { get; set; }

        [StringLength(500)]
        public string? ObservacionesResolucion { get; set; }

        [StringLength(50)]
        public string? TipoRebajo { get; set; }

        public int? AprobadoPorId { get; set; }
        public DateTime? FechaAprobacion { get; set; }

        [StringLength(1000)]
        public string? ComentariosAprobador { get; set; }

        // Navegación
        public virtual Usuario Usuario { get; set; } = null!;
        public virtual Usuario? AprobadoPor { get; set; }
    }
}
