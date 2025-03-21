using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.ViewModels
{
    public class OmisionViewModel
    {
        [Required(ErrorMessage = "La fecha de omisión es obligatoria")]
        [Display(Name = "Fecha de Omisión")]
        [DataType(DataType.Date)]
        public DateTime FechaOmision { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "El puesto es obligatorio")]
        [Display(Name = "Puesto")]
        public string Puesto { get; set; }

        [Required(ErrorMessage = "La instancia es obligatoria")]
        [Display(Name = "Instancia")]
        public string Instancia { get; set; }

        [Required(ErrorMessage = "La categoría de personal es obligatoria")]
        [Display(Name = "Categoría de Personal")]
        public string CategoriaPersonal { get; set; } // Personal docente, Personal administrativo

        [Required(ErrorMessage = "El título es obligatorio")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } // Título I, Título II

        [Required(ErrorMessage = "El tipo de omisión es obligatorio")]
        [Display(Name = "Tipo de Omisión")]
        public string TipoOmision { get; set; } // Entrada, Salida, Todo el día, Salida anticipada

        [Required(ErrorMessage = "La justificación es obligatoria")]
        [Display(Name = "Justificación")]
        public string Motivo { get; set; }
    }
}

