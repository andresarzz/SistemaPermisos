using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.ViewModels
{
    public class ResolucionReporteViewModel
    {
        public int ReporteId { get; set; }

        [Required(ErrorMessage = "El nuevo estado es obligatorio.")]
        [StringLength(50, ErrorMessage = "El estado no puede exceder los 50 caracteres.")]
        [Display(Name = "Nuevo Estado")]
        public string NuevoEstado { get; set; } = string.Empty; // Ej. "En Proceso", "Resuelto", "Rechazado"

        [StringLength(500, ErrorMessage = "Los comentarios no pueden exceder los 500 caracteres.")]
        [Display(Name = "Comentarios de Resolución")]
        public string? ComentariosResolucion { get; set; }
    }
}
