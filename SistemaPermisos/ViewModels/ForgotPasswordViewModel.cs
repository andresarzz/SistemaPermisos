using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [Display(Name = "Correo Electrónico")]
        public string Correo { get; set; }
    }
}
