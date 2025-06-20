using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.ViewModels
{
    public class ResolucionOmisionViewModel
    {
        public int OmisionId { get; set; }

        [Required(ErrorMessage = "La resolución es obligatoria")]
        [Display(Name = "Resolución")]
        public string Resolucion { get; set; } = string.Empty;

        [Display(Name = "Observaciones")]
        public string? ObservacionesResolucion { get; set; }

        // Propiedades adicionales para mostrar información de la omisión
        [Display(Name = "Tipo de Omisión")]
        public string TipoOmision { get; set; } = string.Empty;

        [Display(Name = "Fecha de Omisión")]
        public DateTime FechaOmision { get; set; }

        [Display(Name = "Motivo")]
        public string Motivo { get; set; } = string.Empty;

        [Display(Name = "Nombre del Solicitante")]
        public string NombreSolicitante { get; set; } = string.Empty;

        [Display(Name = "Cédula")]
        public string? Cedula { get; set; }

        [Display(Name = "Puesto")]
        public string? Puesto { get; set; }

        [Display(Name = "Instancia")]
        public string? Instancia { get; set; }

        [Display(Name = "Categoría Personal")]
        public string? CategoriaPersonal { get; set; }

        [Display(Name = "Título")]
        public string? Titulo { get; set; }
    }
}
