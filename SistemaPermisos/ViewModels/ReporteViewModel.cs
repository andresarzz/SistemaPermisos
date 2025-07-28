using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.ViewModels
{
    public class ReporteViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El tipo de daño es obligatorio.")]
        [StringLength(100, ErrorMessage = "El tipo de daño no puede exceder los 100 caracteres.")]
        [Display(Name = "Tipo de Daño")]
        public string TipoDano { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder los 1000 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha del reporte es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha del Reporte")]
        public DateTime FechaReporte { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "El equipo es obligatorio.")]
        [StringLength(255, ErrorMessage = "El nombre del equipo no puede exceder los 255 caracteres.")]
        [Display(Name = "Equipo")]
        public string Equipo { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La ubicación no puede exceder los 200 caracteres.")]
        [Display(Name = "Ubicación")]
        public string? Ubicacion { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder los 500 caracteres.")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [Display(Name = "Evidencia (Imagen/PDF)")]
        public IFormFile? Evidencia { get; set; } // Para subir un archivo de evidencia

        [StringLength(50)]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Pendiente"; // "Pendiente", "En Proceso", "Resuelto", "Rechazado"
    }
}
