using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.Models
{
    public class ReporteDano
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(100)]
        public string Equipo { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Ubicacion { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente";

        public DateTime FechaReporte { get; set; } = DateTime.Now;

        public virtual Usuario? Usuario { get; set; }
    }
}
