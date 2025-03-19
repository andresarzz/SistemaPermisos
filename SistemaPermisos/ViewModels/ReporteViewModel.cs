using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SistemaPermisos.ViewModels
{
    public class ReporteViewModel
    {
        [Required(ErrorMessage = "La ubicación es obligatoria")]
        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [Display(Name = "Descripción del Daño")]
        public string Descripcion { get; set; }

        [Display(Name = "Imagen del Daño")]
        public IFormFile Imagen { get; set; }
    }
}

