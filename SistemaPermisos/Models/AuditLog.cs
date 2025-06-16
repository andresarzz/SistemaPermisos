using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPermisos.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        public int? UsuarioId { get; set; }

        [Required]
        [StringLength(50)]
        public string Accion { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Entidad { get; set; } = string.Empty;

        public int? RegistroId { get; set; }

        public string? ValoresAnteriores { get; set; }

        public string? ValoresNuevos { get; set; }

        [StringLength(45)]
        public string? DireccionIP { get; set; }

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        // Propiedades adicionales que faltan
        public DateTime FechaActividad
        {
            get => Fecha;
            set => Fecha = value;
        }

        public string TipoEntidad
        {
            get => Entidad;
            set => Entidad = value;
        }

        // Navegación
        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }

        // Propiedades de compatibilidad
        public string? DatosAntiguos
        {
            get => ValoresAnteriores;
            set => ValoresAnteriores = value;
        }

        public string? DatosNuevos
        {
            get => ValoresNuevos;
            set => ValoresNuevos = value;
        }

        public string Tabla
        {
            get => Entidad;
            set => Entidad = value;
        }

        public int? EntidadId
        {
            get => RegistroId;
            set => RegistroId = value;
        }
    }
}
