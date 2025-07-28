using System.ComponentModel.DataAnnotations;

namespace SistemaPermisos.Models
{
    public class UserPermission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string PermissionName { get; set; } = string.Empty; // Ej. "Admin", "Supervisor", "Docente"
    }
}
