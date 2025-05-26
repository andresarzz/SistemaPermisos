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

        [Display(Name = "Hora desde")]
        public string HoraDesde { get; set; }

        [Display(Name = "Hora hasta")]
        public string HoraHasta { get; set; }

        [Display(Name = "Jornada completa")]
        public bool JornadaCompleta { get; set; }

        [Display(Name = "Media jornada")]
        public bool MediaJornada { get; set; }

        [Display(Name = "Cantidad de lecciones")]
        public int? CantidadLecciones { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "El puesto es obligatorio")]
        [Display(Name = "Puesto")]
        public string Puesto { get; set; }

        [Required(ErrorMessage = "La condición es obligatoria")]
        [Display(Name = "Condición")]
        public string Condicion { get; set; }

        [Required(ErrorMessage = "El tipo de motivo es obligatorio")]
        [Display(Name = "Tipo de motivo")]
        public string TipoMotivo { get; set; }

        [Display(Name = "Tipo de convocatoria")]
        public string TipoConvocatoria { get; set; }

        [Display(Name = "Motivo")]
        public string Motivo { get; set; }

        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; }

        [Display(Name = "Hora de salida")]
        public string HoraSalida { get; set; }

        [Display(Name = "Comprobante")]
        public IFormFile Comprobante { get; set; }
    }
}
