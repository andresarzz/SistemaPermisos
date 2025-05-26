using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.ViewModels
{
    public class UserEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        [Display(Name = "Nombre completo")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres")]
        [Display(Name = "Correo electrónico")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio")]
        [Display(Name = "Rol")]
        public string Rol { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "La cédula no puede exceder los 20 caracteres")]
        [Display(Name = "Cédula")]
        public string? Cedula { get; set; }

        [StringLength(100, ErrorMessage = "El puesto no puede exceder los 100 caracteres")]
        [Display(Name = "Puesto")]
        public string? Puesto { get; set; }

        [StringLength(15, ErrorMessage = "El teléfono no puede exceder los 15 caracteres")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [StringLength(100, ErrorMessage = "El departamento no puede exceder los 100 caracteres")]
        [Display(Name = "Departamento")]
        public string? Departamento { get; set; }

        [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres")]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistro { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña (opcional)")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nueva contraseña")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string? ConfirmPassword { get; set; }
    }
}
