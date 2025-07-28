using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.ViewModels
{
    public class JustificarPermisoViewModel
    {
        public int PermisoId { get; set; }

        [Required(ErrorMessage = "El motivo de la justificación es obligatorio.")]
        [StringLength(500, ErrorMessage = "El motivo no puede exceder los 500 caracteres.")]
        [Display(Name = "Motivo de Justificación")]
        public string Motivo { get; set; } = string.Empty;

        [Display(Name = "Documento de Justificación")]
        [Required(ErrorMessage = "Debe adjuntar un documento de justificación.")]
        public IFormFile DocumentoJustificacion { get; set; } = null!; // Para subir un archivo
    }
}
