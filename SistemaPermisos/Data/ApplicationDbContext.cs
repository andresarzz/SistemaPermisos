using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Models;

namespace SistemaPermisos.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<OmisionMarca> OmisionesMarca { get; set; }
        public DbSet<ReporteDano> ReportesDano { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<TwoFactorAuth> TwoFactorAuths { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Rol).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configuración de Permiso
            modelBuilder.Entity<Permiso>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(50);
                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de OmisionMarca
            modelBuilder.Entity<OmisionMarca>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(50);
                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de ReporteDano
            modelBuilder.Entity<ReporteDano>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(50);
                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de AuditLog
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Accion).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Entidad).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DireccionIP).HasMaxLength(45);
                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Configuración de UserPermission
            modelBuilder.Entity<UserPermission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de PasswordReset
            modelBuilder.Entity<PasswordReset>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de TwoFactorAuth
            modelBuilder.Entity<TwoFactorAuth>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
