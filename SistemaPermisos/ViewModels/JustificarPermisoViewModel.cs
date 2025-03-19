using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SistemaPermisos.ViewModels
{
    public class JustificarPermisoViewModel
    {
        public int PermisoId { get; set; }

        [Required(ErrorMessage = "La justificación es obligatoria")]
        [Display(Name = "Documento de Justificación")]
        public IFormFile Justificacion { get; set; }
    }
}

