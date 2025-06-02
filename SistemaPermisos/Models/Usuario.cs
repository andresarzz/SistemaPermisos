using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPermisos.Models
{
    public class Usuario
    {
        public Usuario()
        {
            Permisos = new List<Permiso>();
            OmisionesMarca = new List<OmisionMarca>();
            ReportesDanos = new List<ReporteDano>();
            UserPermissions = new List<UserPermission>();
            PasswordResets = new List<PasswordReset>();
            AuditLogs = new List<AuditLog>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Rol { get; set; } = "Docente";

        public bool Activo { get; set; } = true;
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public DateTime UltimaActualizacion { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public virtual ICollection<Permiso> Permisos { get; set; }
        public virtual ICollection<OmisionMarca> OmisionesMarca { get; set; }
        public virtual ICollection<ReporteDano> ReportesDanos { get; set; }
        public virtual ICollection<UserPermission> UserPermissions { get; set; }
        public virtual ICollection<PasswordReset> PasswordResets { get; set; }
        public virtual ICollection<AuditLog> AuditLogs { get; set; }
        public virtual TwoFactorAuth? TwoFactorAuth { get; set; }
    }
}
