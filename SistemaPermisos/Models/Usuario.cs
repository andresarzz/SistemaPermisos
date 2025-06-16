using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Apellidos { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        [StringLength(100)]
        public string Correo { get; set; } = string.Empty;

        // Alias para compatibilidad
        public string Email => Correo;

        [StringLength(50)]
        public string? NombreUsuario { get; set; }

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio")]
        [StringLength(50)]
        public string Rol { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Cedula { get; set; }

        [StringLength(100)]
        public string? Puesto { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(100)]
        public string? Departamento { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        [StringLength(500)]
        public string? FotoPerfil { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public DateTime UltimaActualizacion { get; set; } = DateTime.Now;

        public DateTime? UltimoAcceso { get; set; }

        // Navegación
        public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
        public virtual ICollection<Permiso> Permisos { get; set; } = new List<Permiso>();
        public virtual ICollection<OmisionMarca> OmisionesMarca { get; set; } = new List<OmisionMarca>();
        public virtual ICollection<ReporteDano> ReportesDanos { get; set; } = new List<ReporteDano>();
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        public virtual ICollection<PasswordReset> PasswordResets { get; set; } = new List<PasswordReset>();
        public virtual TwoFactorAuth? TwoFactorAuth { get; set; }
    }
}
