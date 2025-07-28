using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.ViewModels
{
    public class PermisoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El tipo de permiso es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Tipo de Permiso")]
        public string TipoPermiso { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Fin")]
        public DateTime FechaFin { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Hora Desde")]
        public TimeSpan? HoraDesde { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Hora Hasta")]
        public TimeSpan? HoraHasta { get; set; }

        [Display(Name = "Jornada Completa")]
        public bool JornadaCompleta { get; set; } = false;

        [Display(Name = "Media Jornada")]
        public bool MediaJornada { get; set; } = false;

        [Display(Name = "Cantidad de Lecciones")]
        public int? CantidadLecciones { get; set; }

        [StringLength(20)]
        [Display(Name = "Cédula")]
        public string? Cedula { get; set; }

        [StringLength(100)]
        [Display(Name = "Puesto")]
        public string? Puesto { get; set; }

        [StringLength(50)]
        [Display(Name = "Condición")]
        public string? Condicion { get; set; }

        [StringLength(100)]
        [Display(Name = "Tipo de Motivo")]
        public string? TipoMotivo { get; set; }

        [StringLength(100)]
        [Display(Name = "Tipo de Convocatoria")]
        public string? TipoConvocatoria { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio")]
        [StringLength(500)]
        [Display(Name = "Motivo")]
        public string Motivo { get; set; } = string.Empty;

        [Display(Name = "Documento Adjunto (PDF/Imagen)")]
        public IFormFile? DocumentoAdjunto { get; set; } // Para subir un archivo
    }
}
