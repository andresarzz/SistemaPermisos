using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.Models
{
    public class OmisionMarca
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoOmision { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Motivo { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente";

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public virtual Usuario? Usuario { get; set; }
    }
}
