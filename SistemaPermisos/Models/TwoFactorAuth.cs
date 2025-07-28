using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPermisos.Models
{
    public class TwoFactorAuth
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual Usuario User { get; set; } = null!;

        [Required]
        [StringLength(10)]
        public string Code { get; set; } = string.Empty; // The 2FA code

        [Required]
        public DateTime ExpirationDate { get; set; } // When the code expires

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
