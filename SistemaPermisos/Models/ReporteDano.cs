using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.Models
{
    public class ReporteDano
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El equipo es obligatorio")]
        [StringLength(100)]
        public string Equipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de daño es obligatorio")]
        [StringLength(100)]
        public string TipoDano { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(1000)]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ubicación es obligatoria")]
        [StringLength(200)]
        public string Ubicacion { get; set; } = string.Empty;

        public DateTime FechaReporte { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente";

        public DateTime? FechaResolucion { get; set; }

        [StringLength(1000)]
        public string? ComentariosResolucion { get; set; }

        [StringLength(500)]
        public string? RutaImagen { get; set; }

        [StringLength(500)]
        public string? UrlFoto { get; set; }

        public decimal? CostoEstimado { get; set; }

        [StringLength(100)]
        public string? ResueltoPor { get; set; }

        // Navegación
        public virtual Usuario Usuario { get; set; } = null!;
        public virtual Usuario? AsignadoAUsuario { get; set; }
    }
}
