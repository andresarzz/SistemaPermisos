#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPermisos.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Rol { get; set; } = "Usuario";

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

        public DateTime? FechaNacimiento { get; set; }

        [StringLength(255)]
        public string? FotoPerfil { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public DateTime UltimaActualizacion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public virtual ICollection<Permiso> Permisos { get; set; } = new List<Permiso>();
        public virtual ICollection<OmisionMarca> OmisionesMarca { get; set; } = new List<OmisionMarca>();
        public virtual ICollection<ReporteDano> ReportesDanos { get; set; } = new List<ReporteDano>();
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
        public virtual ICollection<PasswordReset> PasswordResets { get; set; } = new List<PasswordReset>();
        public virtual TwoFactorAuth? TwoFactorAuth { get; set; }
    }
}
