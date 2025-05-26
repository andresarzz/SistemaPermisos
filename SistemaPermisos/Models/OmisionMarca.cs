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
        public virtual Usuario Usuario { get; set; }

        [Required(ErrorMessage = "La fecha de la omisión es obligatoria")]
        [Display(Name = "Fecha de Omisión")]
        [DataType(DataType.Date)]
        public DateTime FechaOmision { get; set; }

        [Display(Name = "Cédula")]
        public string Cedula { get; set; }

        [Display(Name = "Puesto")]
        public string Puesto { get; set; }

        [Display(Name = "Instancia")]
        public string Instancia { get; set; }

        [Display(Name = "Categoría de Personal")]
        public string CategoriaPersonal { get; set; } // Personal docente, Personal administrativo

        [Display(Name = "Título")]
        public string Titulo { get; set; } // Título I, Título II

        [Required(ErrorMessage = "El tipo de omisión es obligatorio")]
        [Display(Name = "Tipo de Omisión")]
        public string TipoOmision { get; set; } // Entrada, Salida, Todo el día, Salida anticipada

        [Required(ErrorMessage = "El motivo es obligatorio")]
        [Display(Name = "Justificación")]
        public string Motivo { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Aprobado, Rechazado

        [Display(Name = "Resolución")]
        public string Resolucion { get; set; } // Aceptar con rebajo salarial parcial, Aceptar con rebajo salarial total, Aceptar sin rebajo salarial, Denegar lo solicitado, Acoger convocatoria

        [Display(Name = "Observaciones de Resolución")]
        public string ObservacionesResolucion { get; set; }

        [Display(Name = "Fecha de Registro")]
        [DataType(DataType.DateTime)]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
