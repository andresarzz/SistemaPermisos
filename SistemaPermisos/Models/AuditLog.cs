using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPermisos.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Accion { get; set; } = string.Empty; // Ej. "Login", "Logout", "Crear Permiso", "Aprobar Permiso"

        [StringLength(500)]
        public string? Detalles { get; set; }

        [Required]
        public DateTime FechaHora { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string? IpAddress { get; set; }

        [StringLength(255)]
        public string? UserAgent { get; set; }
    }
}
