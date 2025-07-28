using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPermisos.Models
{
    public class OmisionMarca
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string TipoOmision { get; set; } = string.Empty; // Ej. "Entrada", "Salida"

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaOmision { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan HoraOmision { get; set; }

        [StringLength(500)]
        public string Motivo { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Pendiente"; // Ej. "Pendiente", "Aprobado", "Rechazado"

        [DataType(DataType.Date)]
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? ObservacionesResolucion { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaResolucion { get; set; }

        // Nuevas propiedades añadidas para la información del usuario
        [StringLength(50)]
        public string? Cedula { get; set; }

        [StringLength(100)]
        public string? Puesto { get; set; }

        [StringLength(100)]
        public string? Instancia { get; set; }

        [StringLength(100)]
        public string? CategoriaPersonal { get; set; }

        [StringLength(255)]
        public string? Titulo { get; set; } // Título de la omisión, si aplica

        [StringLength(500)]
        public string? Resolucion { get; set; } // Texto de la resolución

        [StringLength(255)]
        public string? RutaEvidencia { get; set; } // Ruta al archivo de evidencia (imagen, video, etc.)

        public int? AprobadoPorId { get; set; }

        [ForeignKey("AprobadoPorId")]
        public virtual Usuario? AprobadoPor { get; set; }

        public DateTime? FechaAprobacion { get; set; }

        [StringLength(1000)]
        public string? ComentariosAprobador { get; set; }
    }
}
