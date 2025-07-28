using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.ViewModels
{
    public class ResolucionPermisoViewModel
    {
        public int PermisoId { get; set; }

        [Required(ErrorMessage = "El nuevo estado es obligatorio.")]
        [StringLength(50, ErrorMessage = "El estado no puede exceder los 50 caracteres.")]
        [Display(Name = "Nuevo Estado")]
        public string NuevoEstado { get; set; } = string.Empty; // Ej. "Aprobado", "Rechazado"

        [StringLength(500, ErrorMessage = "Los comentarios no pueden exceder los 500 caracteres.")]
        [Display(Name = "Comentarios del Aprobador")]
        public string? ComentariosAprobador { get; set; }

        [StringLength(50, ErrorMessage = "El tipo de rebajo no puede exceder los 50 caracteres.")]
        [Display(Name = "Tipo de Rebajo")]
        public string? TipoRebajo { get; set; } // Ej. "Vacaciones", "Días Libres", "Sin Goce de Salario"
    }
}
