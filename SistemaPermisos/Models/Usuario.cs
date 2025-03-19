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

        // Relaciones
        public virtual ICollection<Permiso> Permisos { get; set; }
        public virtual ICollection<OmisionMarca> OmisionesMarca { get; set; }
        public virtual ICollection<ReporteDano> ReportesDanos { get; set; }
    }
}

