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

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<OmisionMarca> OmisionesMarca { get; set; }
        public DbSet<ReporteDano> ReportesDanos { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<TwoFactorAuth> TwoFactorAuth { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Usuario
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Correo)
                .IsUnique();

            // Configuración de Permiso
            modelBuilder.Entity<Permiso>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Permisos)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de OmisionMarca
            modelBuilder.Entity<OmisionMarca>()
                .HasOne(o => o.Usuario)
                .WithMany(u => u.OmisionesMarca)
                .HasForeignKey(o => o.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de ReporteDano
            modelBuilder.Entity<ReporteDano>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.ReportesDanos)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de AuditLog
            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configuración de UserPermission
            modelBuilder.Entity<UserPermission>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.UserPermissions)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de PasswordReset
            modelBuilder.Entity<PasswordReset>()
                .HasOne(p => p.Usuario)
                .WithMany()
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de TwoFactorAuth
            modelBuilder.Entity<TwoFactorAuth>()
                .HasOne(t => t.Usuario)
                .WithOne()
                .HasForeignKey<TwoFactorAuth>(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
