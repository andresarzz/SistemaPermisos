using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPermisos.Models
{
    public class ReporteDano
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;

        [Required(ErrorMessage = "El equipo es obligatorio")]
        [StringLength(100, ErrorMessage = "El equipo no puede exceder los 100 caracteres")]
        [Display(Name = "Equipo")]
        public string Equipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ubicación es obligatoria")]
        [StringLength(100, ErrorMessage = "La ubicación no puede exceder los 100 caracteres")]
        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "La ruta de imagen no puede exceder los 255 caracteres")]
        [Display(Name = "Ruta de Imagen")]
        public string? RutaImagen { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "El estado no puede exceder los 20 caracteres")]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Pendiente";

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha de Reporte")]
        public DateTime FechaReporte { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha de Resolución")]
        public DateTime? FechaResolucion { get; set; }

        [StringLength(100, ErrorMessage = "El campo 'Resuelto Por' no puede exceder los 100 caracteres")]
        [Display(Name = "Resuelto Por")]
        public string? ResueltoPor { get; set; }
    }
}
