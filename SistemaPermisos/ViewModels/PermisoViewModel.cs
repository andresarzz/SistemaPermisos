using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SistemaPermisos.ViewModels
{
    public class PermisoViewModel
    {
        [Required(ErrorMessage = "La fecha de salida es obligatoria")]
        [Display(Name = "Fecha de Salida")]
        [DataType(DataType.DateTime)]
        public DateTime FechaSalida { get; set; }

        [Required(ErrorMessage = "La fecha de regreso es obligatoria")]
        [Display(Name = "Fecha de Regreso")]
        [DataType(DataType.DateTime)]
        public DateTime FechaRegreso { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio")]
        [Display(Name = "Motivo")]
        public string Motivo { get; set; }

        [Display(Name = "Comprobante (imagen)")]
        public IFormFile Comprobante { get; set; }
    }
}

