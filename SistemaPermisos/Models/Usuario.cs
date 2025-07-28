using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace SistemaPermisos.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Apellidos { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Cedula { get; set; }

        [StringLength(100)]
        public string? Puesto { get; set; }

        [StringLength(15)]
        public string? Telefono { get; set; }

        [StringLength(100)]
        public string? Departamento { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaNacimiento { get; set; }

        [Required]
        [StringLength(50)]
        public string Rol { get; set; } = "Docente"; // Ej. "Admin", "Supervisor", "Docente"

        public bool IsActive { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public DateTime UltimaActualizacion { get; set; } = DateTime.Now;

        public DateTime? UltimoAcceso { get; set; }

        public bool EmailConfirmed { get; set; } = false;

        [StringLength(255)]
        public string? FotoPerfilUrl { get; set; } // Ruta a la imagen de perfil

        // Navegación
        public virtual ICollection<Permiso> PermisosSolicitados { get; set; } = new List<Permiso>();
        public virtual ICollection<OmisionMarca> OmisionesSolicitadas { get; set; } = new List<OmisionMarca>();
        public virtual ICollection<ReporteDano> ReportesCreados { get; set; } = new List<ReporteDano>();
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }
}
