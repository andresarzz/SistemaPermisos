using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.ViewModels
{
    public class ReporteViewModel
    {
        [Required(ErrorMessage = "El equipo es obligatorio")]
        [StringLength(100, ErrorMessage = "El equipo no puede tener más de 100 caracteres")]
        [Display(Name = "Equipo")]
        public string Equipo { get; set; }

        [Required(ErrorMessage = "La ubicación es obligatoria")]
        [StringLength(100, ErrorMessage = "La ubicación no puede tener más de 100 caracteres")]
        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
        [Display(Name = "Descripción del Daño")]
        public string Descripcion { get; set; }

        [Display(Name = "Imagen (opcional)")]
        public IFormFile Imagen { get; set; }
    }
}
