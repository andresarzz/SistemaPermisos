using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.Models
{
    public class OmisionMarca
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "La fecha de la omisión es obligatoria")]
        [Display(Name = "Fecha de Omisión")]
        [DataType(DataType.DateTime)]
        public DateTime FechaOmision { get; set; }

        [Required(ErrorMessage = "El tipo de omisión es obligatorio")]
        [Display(Name = "Tipo de Omisión")]
        public string TipoOmision { get; set; } // Entrada o Salida

        [Required(ErrorMessage = "El motivo es obligatorio")]
        [Display(Name = "Motivo")]
        public string Motivo { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Aprobado, Rechazado

        [Display(Name = "Fecha de Registro")]
        [DataType(DataType.DateTime)]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Relaciones
        public virtual Usuario Usuario { get; set; }
    }
}

