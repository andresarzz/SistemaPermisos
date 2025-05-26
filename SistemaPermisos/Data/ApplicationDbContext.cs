#nullable enable
using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Models;

namespace SistemaPermisos.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Permiso> Permisos { get; set; } = null!;
        public DbSet<OmisionMarca> OmisionesMarca { get; set; } = null!;
        public DbSet<ReporteDano> ReportesDanos { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<UserPermission> UserPermissions { get; set; } = null!;
        public DbSet<PasswordReset> PasswordResets { get; set; } = null!;
        public DbSet<TwoFactorAuth> TwoFactorAuths { get; set; } = null!;

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
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Rol).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FechaRegistro).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UltimaActualizacion).HasDefaultValueSql("GETDATE()");
            });

            // Configuración de Permiso
            modelBuilder.Entity<Permiso>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                      .WithMany(u => u.Permisos)
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Motivo).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(20);
                entity.Property(e => e.FechaSolicitud).HasDefaultValueSql("GETDATE()");
            });

            // Configuración de OmisionMarca
            modelBuilder.Entity<OmisionMarca>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                      .WithMany(u => u.OmisionesMarca)
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.TipoOmision).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Motivo).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(20);
                entity.Property(e => e.FechaRegistro).HasDefaultValueSql("GETDATE()");
            });

            // Configuración de ReporteDano
            modelBuilder.Entity<ReporteDano>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                      .WithMany(u => u.ReportesDanos)
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Equipo).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Ubicacion).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(20);
                entity.Property(e => e.FechaReporte).HasDefaultValueSql("GETDATE()");
            });

            // Configuración de AuditLog
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                      .WithMany(u => u.AuditLogs)
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.Property(e => e.Accion).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Tabla).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DireccionIP).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Fecha).HasDefaultValueSql("GETDATE()");
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
                entity.Property(e => e.FechaAsignacion).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => new { e.UsuarioId, e.Permiso }).IsUnique();
            });

            // Configuración de PasswordReset
            modelBuilder.Entity<PasswordReset>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                      .WithMany(u => u.PasswordResets)
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Token).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => e.Token).IsUnique();
            });

            // Configuración de TwoFactorAuth
            modelBuilder.Entity<TwoFactorAuth>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                      .WithOne(u => u.TwoFactorAuth)
                      .HasForeignKey<TwoFactorAuth>(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.ClaveSecreta).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FechaActualizacion).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => e.UsuarioId).IsUnique();
            });
        }
    }
}
