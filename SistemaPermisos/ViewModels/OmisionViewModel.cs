using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.ViewModels
{
    public class OmisionViewModel
    {
        [Required(ErrorMessage = "La fecha de omisión es obligatoria")]
        [Display(Name = "Fecha y Hora de Omisión")]
        [DataType(DataType.DateTime)]
        public DateTime FechaOmision { get; set; }

        [Required(ErrorMessage = "El tipo de omisión es obligatorio")]
        [Display(Name = "Tipo de Omisión")]
        public string TipoOmision { get; set; } // Entrada o Salida

        [Required(ErrorMessage = "El motivo es obligatorio")]
        [Display(Name = "Motivo de la Omisión")]
        public string Motivo { get; set; }
    }
}

