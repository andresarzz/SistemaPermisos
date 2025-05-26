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
        public virtual Usuario Usuario { get; set; }

        [Required(ErrorMessage = "El equipo es obligatorio")]
        [StringLength(100)]
        public string Equipo { get; set; }

        [Required(ErrorMessage = "La ubicación es obligatoria")]
        [StringLength(100)]
        public string Ubicacion { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500)]
        public string Descripcion { get; set; }

        [Display(Name = "Ruta de Imagen")]
        public string RutaImagen { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente";

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Reporte")]
        public DateTime FechaReporte { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Resolución")]
        public DateTime? FechaResolucion { get; set; }

        [StringLength(100)]
        [Display(Name = "Resuelto Por")]
        public string ResueltoPor { get; set; }
    }
}
