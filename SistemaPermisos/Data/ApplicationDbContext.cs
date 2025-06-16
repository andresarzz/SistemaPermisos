using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Models;

namespace SistemaPermisos.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Permiso> Permisos => Set<Permiso>();
        public DbSet<OmisionMarca> OmisionesMarca => Set<OmisionMarca>();
        public DbSet<ReporteDano> ReportesDanos => Set<ReporteDano>();
        public DbSet<ReporteDano> ReportesDano => Set<ReporteDano>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
        public DbSet<PasswordReset> PasswordResets => Set<PasswordReset>();
        public DbSet<TwoFactorAuth> TwoFactorAuth => Set<TwoFactorAuth>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Correo).IsUnique();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Correo).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.Rol).IsRequired().HasMaxLength(50);
            });

            // Configuración de Permiso
            modelBuilder.Entity<Permiso>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.TipoPermiso).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Motivo).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(20);
            });

            // Configuración de OmisionMarca
            modelBuilder.Entity<OmisionMarca>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.TipoOmision).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Justificacion).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(20);
            });

            // Configuración de ReporteDano
            modelBuilder.Entity<ReporteDano>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.TipoDano).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Ubicacion).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(20);
            });

            // Configuración de AuditLog
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.Property(e => e.Accion).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Entidad).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DireccionIP).HasMaxLength(45);
                entity.Property(e => e.Descripcion).HasMaxLength(1000);
                entity.HasIndex(e => e.Fecha);
            });

            // Configuración de UserPermission
            modelBuilder.Entity<UserPermission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                      .WithMany(u => u.UserPermissions)
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Permiso).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => new { e.UsuarioId, e.Permiso }).IsUnique();
            });

            // Configuración de PasswordReset
            modelBuilder.Entity<PasswordReset>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Token).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Token).IsUnique();
            });

            // Configuración de TwoFactorAuth
            modelBuilder.Entity<TwoFactorAuth>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Tipo).IsRequired().HasMaxLength(20);
            });
        }
    }
}
