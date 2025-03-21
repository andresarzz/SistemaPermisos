using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.ViewModels
{
    public class ResolucionOmisionViewModel
    {
        public int OmisionId { get; set; }

        [Required(ErrorMessage = "La resolución es obligatoria")]
        [Display(Name = "Resolución")]
        public string Resolucion { get; set; } // Aceptar con rebajo salarial parcial, Aceptar con rebajo salarial total, Aceptar sin rebajo salarial, Denegar lo solicitado, Acoger convocatoria

        [Display(Name = "Observaciones")]
        public string? ObservacionesResolucion { get; set; }
    }
}

