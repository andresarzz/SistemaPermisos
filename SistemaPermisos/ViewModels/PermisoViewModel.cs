using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SistemaPermisos.ViewModels
{
    public class PermisoViewModel
    {
        [Required(ErrorMessage = "La fecha es obligatoria")]
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Display(Name = "Hora Desde")]
        [DataType(DataType.Time)]
        public DateTime? HoraDesde { get; set; }

        [Display(Name = "Hora Hasta")]
        [DataType(DataType.Time)]
        public DateTime? HoraHasta { get; set; }

        [Display(Name = "Jornada Laboral Completa")]
        public bool JornadaCompleta { get; set; }

        [Display(Name = "Media Jornada")]
        public bool MediaJornada { get; set; }

        [Display(Name = "Cantidad de Lecciones")]
        public int? CantidadLecciones { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [Display(Name = "Cédula de identidad")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "El puesto es obligatorio")]
        [Display(Name = "Puesto")]
        public string Puesto { get; set; }

        [Required(ErrorMessage = "La condición es obligatoria")]
        [Display(Name = "Condición")]
        public string Condicion { get; set; } // Propietario o Interino

        [Required(ErrorMessage = "El tipo de motivo es obligatorio")]
        [Display(Name = "Motivo de la solicitud")]
        public string TipoMotivo { get; set; } // Cita médica personal, Acompañar a cita médica, Asistencia a Convocatoria, Asuntos personales

        [Display(Name = "Tipo de Convocatoria")]
        public string? TipoConvocatoria { get; set; } // Sindical, Regional, Nacional

        [Display(Name = "Especifique (para asuntos personales)")]
        public string? Motivo { get; set; }

        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [Display(Name = "Hora de Salida")]
        public string? HoraSalida { get; set; }

        [Display(Name = "Comprobante (imagen)")]
        public IFormFile? Comprobante { get; set; }
    }
}

