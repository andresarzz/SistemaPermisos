using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.ViewModels
{
    public class ResolucionPermisoViewModel
    {
        public int PermisoId { get; set; }

        [Required(ErrorMessage = "La resolución es obligatoria")]
        [Display(Name = "Resolución")]
        public string Resolucion { get; set; } = string.Empty;

        [Display(Name = "Observaciones")]
        public string? ObservacionesResolucion { get; set; }

        [Display(Name = "Tipo de Rebajo")]
        public string? TipoRebajo { get; set; }
    }
}
