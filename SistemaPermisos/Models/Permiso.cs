using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPermisos.Models
{
    public class Permiso
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El usuario es requerido")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El motivo es requerido")]
        [StringLength(500, ErrorMessage = "El motivo no puede exceder los 500 caracteres")]
        [Display(Name = "Motivo")]
        public string Motivo { get; set; }

        [Required(ErrorMessage = "El estado es requerido")]
        [StringLength(20, ErrorMessage = "El estado no puede exceder los 20 caracteres")]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Aprobado, Rechazado

        [Display(Name = "Justificado")]
        public bool Justificado { get; set; } = false;

        [Display(Name = "Fecha de Solicitud")]
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        [StringLength(500)]
        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; }

        [Display(Name = "Hora Desde")]
        public TimeSpan? HoraDesde { get; set; }

        [Display(Name = "Hora Hasta")]
        public TimeSpan? HoraHasta { get; set; }

        [Display(Name = "Jornada Completa")]
        public bool JornadaCompleta { get; set; }

        [Display(Name = "Media Jornada")]
        public bool MediaJornada { get; set; }

        [Display(Name = "Cantidad de Lecciones")]
        public int? CantidadLecciones { get; set; }

        [Display(Name = "Cédula")]
        public string Cedula { get; set; }

        [Display(Name = "Puesto")]
        public string Puesto { get; set; }

        [Display(Name = "Condición")]
        public string Condicion { get; set; }

        [Display(Name = "Tipo de Motivo")]
        public string TipoMotivo { get; set; }

        [Display(Name = "Tipo de Convocatoria")]
        public string TipoConvocatoria { get; set; }

        [Display(Name = "Hora de Salida")]
        public TimeSpan? HoraSalida { get; set; }

        [Display(Name = "Ruta del Comprobante")]
        public string RutaComprobante { get; set; }

        [Display(Name = "Ruta de Justificación")]
        public string RutaJustificacion { get; set; }

        // Métodos de extensión
        public string Resolucion(Permiso permiso)
        {
            return permiso.Estado == "Aprobado" ? "Aprobado" : "Rechazado";
        }

        public string ObservacionesResolucion(Permiso permiso)
        {
            return permiso.Observaciones ?? "Sin observaciones";
        }

        public string TipoRebajo(Permiso permiso)
        {
            if (permiso.JornadaCompleta)
                return "Jornada Completa";
            else if (permiso.MediaJornada)
                return "Media Jornada";
            else if (permiso.CantidadLecciones.HasValue && permiso.CantidadLecciones.Value > 0)
                return $"{permiso.CantidadLecciones} Lecciones";
            else
                return "No Aplica";
        }
    }
}
