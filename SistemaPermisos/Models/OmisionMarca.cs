using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPermisos.Models
{
    public class OmisionMarca
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;

        [Required(ErrorMessage = "La fecha de la omisión es obligatoria")]
        [Display(Name = "Fecha de Omisión")]
        [DataType(DataType.Date)]
        public DateTime FechaOmision { get; set; }

        [StringLength(20, ErrorMessage = "La cédula no puede exceder los 20 caracteres")]
        [Display(Name = "Cédula")]
        public string? Cedula { get; set; }

        [StringLength(100, ErrorMessage = "El puesto no puede exceder los 100 caracteres")]
        [Display(Name = "Puesto")]
        public string? Puesto { get; set; }

        [StringLength(100, ErrorMessage = "La instancia no puede exceder los 100 caracteres")]
        [Display(Name = "Instancia")]
        public string? Instancia { get; set; }

        [StringLength(50, ErrorMessage = "La categoría de personal no puede exceder los 50 caracteres")]
        [Display(Name = "Categoría de Personal")]
        public string? CategoriaPersonal { get; set; }

        [StringLength(50, ErrorMessage = "El título no puede exceder los 50 caracteres")]
        [Display(Name = "Título")]
        public string? Titulo { get; set; }

        [Required(ErrorMessage = "El tipo de omisión es obligatorio")]
        [StringLength(50, ErrorMessage = "El tipo de omisión no puede exceder los 50 caracteres")]
        [Display(Name = "Tipo de Omisión")]
        public string TipoOmision { get; set; } = string.Empty;

        [Required(ErrorMessage = "El motivo es obligatorio")]
        [StringLength(500, ErrorMessage = "El motivo no puede exceder los 500 caracteres")]
        [Display(Name = "Justificación")]
        public string Motivo { get; set; } = string.Empty;

        [Required]
        [StringLength(20, ErrorMessage = "El estado no puede exceder los 20 caracteres")]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Pendiente";

        [StringLength(100, ErrorMessage = "La resolución no puede exceder los 100 caracteres")]
        [Display(Name = "Resolución")]
        public string? Resolucion { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones de resolución no pueden exceder los 500 caracteres")]
        [Display(Name = "Observaciones de Resolución")]
        public string? ObservacionesResolucion { get; set; }

        [Required]
        [Display(Name = "Fecha de Registro")]
        [DataType(DataType.DateTime)]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
