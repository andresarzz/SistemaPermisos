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

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Display(Name = "Hora Desde")]
        [DataType(DataType.Time)]
        public DateTime? HoraDesde { get; set; }

        [Display(Name = "Hora Hasta")]
        [DataType(DataType.Time)]
        public DateTime? HoraHasta { get; set; }

        [Display(Name = "Jornada Completa")]
        public bool JornadaCompleta { get; set; }

        [Display(Name = "Media Jornada")]
        public bool MediaJornada { get; set; }

        [Display(Name = "Cantidad de Lecciones")]
        public int? CantidadLecciones { get; set; }

        [Display(Name = "Cédula")]
        public string? Cedula { get; set; }

        [Display(Name = "Puesto")]
        public string? Puesto { get; set; }

        [Display(Name = "Condición")]
        public string? Condicion { get; set; } // Propietario o Interino

        [Required(ErrorMessage = "El motivo es obligatorio")]
        [Display(Name = "Motivo")]
        public string Motivo { get; set; }

        [Display(Name = "Tipo de Motivo")]
        public string? TipoMotivo { get; set; } // Cita médica personal, Acompañar a cita médica, Asistencia a Convocatoria, Asuntos personales

        [Display(Name = "Tipo de Convocatoria")]
        public string? TipoConvocatoria { get; set; } // Sindical, Regional, Nacional

        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [Display(Name = "Hora de Salida")]
        public string? HoraSalida { get; set; }

        [Display(Name = "Ruta de Comprobante")]
        public string? RutaComprobante { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Aprobado, Rechazado

        [Display(Name = "Resolución")]
        public string? Resolucion { get; set; } // Aceptar lo solicitado, Denegar lo solicitado, Acoger convocatoria

        [Display(Name = "Observaciones de Resolución")]
        public string? ObservacionesResolucion { get; set; }

        [Display(Name = "Fecha de Solicitud")]
        [DataType(DataType.DateTime)]
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        [Display(Name = "Justificado")]
        public bool Justificado { get; set; } = false;

        [Display(Name = "Ruta de Justificación")]
        public string? RutaJustificacion { get; set; }

        [Display(Name = "Tipo de Rebajo")]
        public string? TipoRebajo { get; set; } // Con rebajo salarial parcial, Con rebajo salarial total, Sin rebajo salarial

        // Relaciones
        public virtual Usuario Usuario { get; set; }
    }
}

