using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario o correo electrónico es obligatorio")]
        [Display(Name = "Nombre de usuario o Correo electrónico")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
    }
}
