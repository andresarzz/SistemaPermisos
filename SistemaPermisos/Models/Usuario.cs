using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.Models
{
    public class Usuario
    {
        public Usuario()
        {
            // Inicializar colecciones para evitar NullReferenceException
            Permisos = new List<Permiso>();
            OmisionesMarca = new List<OmisionMarca>();
            ReportesDanos = new List<ReporteDano>();
            UserPermissions = new List<UserPermission>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [Display(Name = "Correo Electrónico")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        [Display(Name = "Rol")]
        public string Rol { get; set; }

        [Display(Name = "Cédula")]
        public string Cedula { get; set; }

        [Display(Name = "Puesto")]
        public string Puesto { get; set; }

        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [Display(Name = "Departamento")]
        public string Departamento { get; set; }

        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime? FechaNacimiento { get; set; }

        [Display(Name = "Foto de Perfil")]
        public string FotoPerfil { get; set; }

        [Display(Name = "Fecha de Registro")]
        [DataType(DataType.DateTime)]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Display(Name = "Última Actualización")]
        [DataType(DataType.DateTime)]
        public DateTime UltimaActualizacion { get; set; } = DateTime.Now;

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        // Relaciones
        public virtual ICollection<Permiso> Permisos { get; set; }
        public virtual ICollection<OmisionMarca> OmisionesMarca { get; set; }
        public virtual ICollection<ReporteDano> ReportesDanos { get; set; }
        public virtual ICollection<UserPermission> UserPermissions { get; set; }
    }
}
