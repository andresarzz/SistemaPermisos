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
        [StringLength(100)]
        public string Accion { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Entidad { get; set; } = string.Empty;

        public int? RegistroId { get; set; }

        public string? ValoresAnteriores { get; set; }

        public string? ValoresNuevos { get; set; }

        [StringLength(45)]
        public string? DireccionIP { get; set; }

        public string? Descripcion { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        // Propiedades adicionales para compatibilidad con vistas
        [NotMapped]
        public DateTime FechaActividad
        {
            get => Fecha;
            set => Fecha = value;
        }

        [NotMapped]
        public string TipoEntidad
        {
            get => Entidad;
            set => Entidad = value;
        }

        // Propiedades de compatibilidad (NotMapped para evitar duplicación en BD)
        [NotMapped]
        public string? DatosAntiguos
        {
            get => ValoresAnteriores;
            set => ValoresAnteriores = value;
        }

        [NotMapped]
        public string? DatosNuevos
        {
            get => ValoresNuevos;
            set => ValoresNuevos = value;
        }

        [NotMapped]
        public string Tabla
        {
            get => Entidad;
            set => Entidad = value;
        }

        [NotMapped]
        public int? EntidadId
        {
            get => RegistroId;
            set => RegistroId = value;
        }

        // Navegación
        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }
    }
}
