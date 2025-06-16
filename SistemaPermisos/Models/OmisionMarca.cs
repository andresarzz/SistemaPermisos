using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.Models
{
    public class OmisionMarca
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El tipo de omisión es obligatorio")]
        [StringLength(50)]
        public string TipoOmision { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Fecha { get; set; }

        public DateTime FechaOmision { get; set; }

        [Required(ErrorMessage = "La justificación es obligatoria")]
        [StringLength(500)]
        public string Justificacion { get; set; } = string.Empty;

        // Propiedades adicionales que faltan
        [StringLength(500)]
        public string Motivo { get; set; } = string.Empty;

        [StringLength(20)]
        public string Cedula { get; set; } = string.Empty;

        [StringLength(100)]
        public string Puesto { get; set; } = string.Empty;

        [StringLength(100)]
        public string Instancia { get; set; } = string.Empty;

        [StringLength(50)]
        public string CategoriaPersonal { get; set; } = string.Empty;

        [StringLength(100)]
        public string Titulo { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente";

        [StringLength(100)]
        public string? Resolucion { get; set; }

        [StringLength(500)]
        public string? ObservacionesResolucion { get; set; }

        public int? AprobadoPorId { get; set; }
        public DateTime? FechaAprobacion { get; set; }

        [StringLength(1000)]
        public string? ComentariosAprobador { get; set; }

        // Navegación
        public virtual Usuario Usuario { get; set; } = null!;
        public virtual Usuario? AprobadoPor { get; set; }
    }
}
