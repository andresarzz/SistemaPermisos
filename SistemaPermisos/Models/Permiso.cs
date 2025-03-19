using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPermisos.Models
{
    public class Permiso
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "La fecha de salida es obligatoria")]
        [Display(Name = "Fecha de Salida")]
        [DataType(DataType.DateTime)]
        public DateTime FechaSalida { get; set; }

        [Required(ErrorMessage = "La fecha de regreso es obligatoria")]
        [Display(Name = "Fecha de Regreso")]
        [DataType(DataType.DateTime)]
        public DateTime FechaRegreso { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio")]
        [Display(Name = "Motivo")]
        public string Motivo { get; set; }

        [Display(Name = "Ruta de Comprobante")]
        public string? RutaComprobante { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Aprobado, Rechazado

        [Display(Name = "Fecha de Solicitud")]
        [DataType(DataType.DateTime)]
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        [Display(Name = "Justificado")]
        public bool Justificado { get; set; } = false;

        [Display(Name = "Ruta de Justificación")]
        public string? RutaJustificacion { get; set; }

        // Relaciones
        public virtual Usuario Usuario { get; set; }
    }
}

