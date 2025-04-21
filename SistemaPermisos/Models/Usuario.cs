using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [Display(Name = "Correo Electrónico")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Rol")]
        public string Rol { get; set; } = "Docente"; // Por defecto es Docente, puede ser Admin

        [Display(Name = "Foto de Perfil")]
        public string? FotoPerfil { get; set; }

        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [Display(Name = "Departamento")]
        public string? Departamento { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime? FechaNacimiento { get; set; }

        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [Display(Name = "Cédula")]
        public string? Cedula { get; set; }

        [Display(Name = "Puesto")]
        public string? Puesto { get; set; }

        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Display(Name = "Última Actualización")]
        public DateTime UltimaActualizacion { get; set; } = DateTime.Now;

        // Agregar esta propiedad al modelo Usuario
        [Display(Name = "Estado")]
        public bool Activo { get; set; } = true;

        // Relaciones
        public virtual ICollection<Permiso> Permisos { get; set; }
        public virtual ICollection<OmisionMarca> OmisionesMarca { get; set; }
        public virtual ICollection<ReporteDano> ReportesDanos { get; set; }
    }
}

