using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPermisos.Models
{
    public class Permiso
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;

        [Required(ErrorMessage = "El tipo de permiso es obligatorio")]
        [StringLength(100, ErrorMessage = "El tipo de permiso no puede exceder los 100 caracteres")]
        [Display(Name = "Tipo de Permiso")]
        public string TipoPermiso { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Fin")]
        public DateTime FechaFin { get; set; }

        [Display(Name = "Hora desde")]
        public TimeSpan? HoraDesde { get; set; }

        [Display(Name = "Hora hasta")]
        public TimeSpan? HoraHasta { get; set; }

        [Display(Name = "Jornada completa")]
        public bool JornadaCompleta { get; set; }

        [Display(Name = "Media jornada")]
        public bool MediaJornada { get; set; }

        [Display(Name = "Cantidad de lecciones")]
        public int? CantidadLecciones { get; set; }

        [StringLength(20, ErrorMessage = "La cédula no puede exceder los 20 caracteres")]
        [Display(Name = "Cédula")]
        public string? Cedula { get; set; }

        [StringLength(100, ErrorMessage = "El puesto no puede exceder los 100 caracteres")]
        [Display(Name = "Puesto")]
        public string? Puesto { get; set; }

        [StringLength(50, ErrorMessage = "La condición no puede exceder los 50 caracteres")]
        [Display(Name = "Condición")]
        public string? Condicion { get; set; }

        [StringLength(100, ErrorMessage = "El tipo de motivo no puede exceder los 100 caracteres")]
        [Display(Name = "Tipo de motivo")]
        public string? TipoMotivo { get; set; }

        [StringLength(100, ErrorMessage = "El tipo de convocatoria no puede exceder los 100 caracteres")]
        [Display(Name = "Tipo de convocatoria")]
        public string? TipoConvocatoria { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio")]
        [StringLength(500, ErrorMessage = "El motivo no puede exceder los 500 caracteres")]
        [Display(Name = "Motivo")]
        public string Motivo { get; set; } = string.Empty;

        [Required]
        [StringLength(20, ErrorMessage = "El estado no puede exceder los 20 caracteres")]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Pendiente";

        [Display(Name = "Hora de salida")]
        public string? HoraSalida { get; set; }

        [StringLength(255, ErrorMessage = "La ruta del comprobante no puede exceder los 255 caracteres")]
        [Display(Name = "Ruta del comprobante")]
        public string? RutaComprobante { get; set; }

        [StringLength(255, ErrorMessage = "La ruta de justificación no puede exceder los 255 caracteres")]
        [Display(Name = "Ruta de justificación")]
        public string? RutaJustificacion { get; set; }

        [Display(Name = "Justificado")]
        public bool Justificado { get; set; } = false;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha de Solicitud")]
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder los 500 caracteres")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [StringLength(100, ErrorMessage = "La resolución no puede exceder los 100 caracteres")]
        [Display(Name = "Resolución")]
        public string? Resolucion { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones de resolución no pueden exceder los 500 caracteres")]
        [Display(Name = "Observaciones de resolución")]
        public string? ObservacionesResolucion { get; set; }

        [StringLength(50, ErrorMessage = "El tipo de rebajo no puede exceder los 50 caracteres")]
        [Display(Name = "Tipo de rebajo")]
        public string? TipoRebajo { get; set; }
    }
}
