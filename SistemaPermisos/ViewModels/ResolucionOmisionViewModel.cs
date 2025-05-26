using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.ViewModels
{
    public class ResolucionOmisionViewModel
    {
        public int OmisionId { get; set; }

        [Required(ErrorMessage = "La resolución es obligatoria")]
        [Display(Name = "Resolución")]
        public string Resolucion { get; set; } = string.Empty;

        [Display(Name = "Observaciones")]
        public string? ObservacionesResolucion { get; set; }
    }
}
