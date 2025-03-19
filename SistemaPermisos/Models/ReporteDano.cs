using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.Models
{
    public class ReporteDano
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "La ubicación es obligatoria")]
        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [Display(Name = "Descripción del Daño")]
        public string Descripcion { get; set; }

        [Display(Name = "Ruta de Imagen")]
        public string RutaImagen { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Resuelto

        [Display(Name = "Fecha de Reporte")]
        [DataType(DataType.DateTime)]
        public DateTime FechaReporte { get; set; } = DateTime.Now;

        // Relaciones
        public virtual Usuario Usuario { get; set; }
    }
}

