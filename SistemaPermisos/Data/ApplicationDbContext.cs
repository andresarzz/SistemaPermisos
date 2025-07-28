using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Models;

namespace SistemaPermisos.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Permiso> Permisos { get; set; } = null!;
        public DbSet<OmisionMarca> OmisionesMarca { get; set; } = null!;
        public DbSet<ReporteDano> ReportesDano { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<UserPermission> UserPermissions { get; set; } = null!;
        public DbSet<PasswordReset> PasswordResets { get; set; } = null!;
        public DbSet<TwoFactorAuth> TwoFactorAuths { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Permiso>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.PermisosSolicitados)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            modelBuilder.Entity<Permiso>()
                .HasOne(p => p.AprobadoPor)
                .WithMany()
                .HasForeignKey(p => p.AprobadoPorId)
                .OnDelete(DeleteBehavior.SetNull); // Set null if approver is deleted

            modelBuilder.Entity<OmisionMarca>()
                .HasOne(o => o.Usuario)
                .WithMany(u => u.OmisionesSolicitadas)
                .HasForeignKey(o => o.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OmisionMarca>()
                .HasOne(o => o.AprobadoPor)
                .WithMany()
                .HasForeignKey(o => o.AprobadoPorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ReporteDano>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.ReportesCreados)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReporteDano>()
                .HasOne(r => r.ResueltoPor)
                .WithMany()
                .HasForeignKey(r => r.ResueltoPorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AuditLog>()
                .HasOne(al => al.Usuario)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(al => al.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ensure unique constraints for Email and NombreUsuario
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.NombreUsuario)
                .IsUnique();
        }
    }
}
