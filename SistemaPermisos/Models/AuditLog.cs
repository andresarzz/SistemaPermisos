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

        [StringLength(2000)]
        public string? ValoresAnteriores { get; set; }

        [StringLength(2000)]
        public string? ValoresNuevos { get; set; }

        [Required]
        [StringLength(45)]
        public string DireccionIP { get; set; } = string.Empty;

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;

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
