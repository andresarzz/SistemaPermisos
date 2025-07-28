using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.ViewModels
{
    public class OmisionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El tipo de omisión es obligatorio.")]
        [StringLength(100, ErrorMessage = "El tipo de omisión no puede exceder los 100 caracteres.")]
        [Display(Name = "Tipo de Omisión")]
        public string TipoOmision { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de omisión es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Omisión")]
        public DateTime FechaOmision { get; set; }

        [Required(ErrorMessage = "La hora de omisión es obligatoria.")]
        [DataType(DataType.Time)]
        [Display(Name = "Hora de Omisión")]
        public TimeSpan HoraOmision { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio.")]
        [StringLength(500, ErrorMessage = "El motivo no puede exceder los 500 caracteres.")]
        [Display(Name = "Motivo")]
        public string Motivo { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "La cédula no puede exceder los 20 caracteres.")]
        [Display(Name = "Cédula")]
        public string? Cedula { get; set; }

        [StringLength(100, ErrorMessage = "El puesto no puede exceder los 100 caracteres.")]
        [Display(Name = "Puesto")]
        public string? Puesto { get; set; }

        [StringLength(100, ErrorMessage = "La instancia no puede exceder los 100 caracteres.")]
        [Display(Name = "Instancia")]
        public string? Instancia { get; set; }

        [StringLength(100, ErrorMessage = "La categoría personal no puede exceder los 100 caracteres.")]
        [Display(Name = "Categoría Personal")]
        public string? CategoriaPersonal { get; set; }

        [StringLength(255, ErrorMessage = "El título no puede exceder los 255 caracteres.")]
        [Display(Name = "Título")]
        public string? Titulo { get; set; }

        [Display(Name = "Evidencia (Imagen/PDF)")]
        public IFormFile? Evidencia { get; set; } // Para subir un archivo de evidencia
    }
}
