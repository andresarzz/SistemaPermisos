using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Models;
using System.Threading;
using System.Threading.Tasks;

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

            // Configuraciones adicionales de relaciones
            modelBuilder.Entity<Permiso>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Permisos)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OmisionMarca>()
                .HasOne(o => o.Usuario)
                .WithMany(u => u.OmisionesMarca)
                .HasForeignKey(o => o.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReporteDano>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.ReportesDanos)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<UserPermission>()
                .HasOne(up => up.Usuario)
                .WithMany()
                .HasForeignKey(up => up.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PasswordReset>()
                .HasOne(pr => pr.Usuario)
                .WithMany()
                .HasForeignKey(pr => pr.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TwoFactorAuth>()
                .HasOne(tfa => tfa.Usuario)
                .WithMany()
                .HasForeignKey(tfa => tfa.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Datos iniciales (opcional)
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nombre = "Administrador",
                    Correo = "admin@escuela.edu",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Rol = "Admin",
                    Activo = true
                },
                new Usuario
                {
                    Id = 2,
                    Nombre = "Docente Demo",
                    Correo = "docente@escuela.edu",
                    Password = BCrypt.Net.BCrypt.HashPassword("Docente123!"),
                    Rol = "Docente",
                    Activo = true
                }
            );

            // Permisos predefinidos
            modelBuilder.Entity<UserPermission>().HasData(
                new UserPermission { Id = 1, UsuarioId = 1, Permiso = "usuarios.ver" },
                new UserPermission { Id = 2, UsuarioId = 1, Permiso = "usuarios.crear" },
                new UserPermission { Id = 3, UsuarioId = 1, Permiso = "usuarios.editar" },
                new UserPermission { Id = 4, UsuarioId = 1, Permiso = "usuarios.eliminar" },
                new UserPermission { Id = 5, UsuarioId = 1, Permiso = "permisos.aprobar" },
                new UserPermission { Id = 6, UsuarioId = 1, Permiso = "omisiones.aprobar" },
                new UserPermission { Id = 7, UsuarioId = 1, Permiso = "reportes.resolver" },
                new UserPermission { Id = 8, UsuarioId = 1, Permiso = "auditoria.ver" }
            );
        }

        // Sobrescribir SaveChanges para implementar auditoría automática
        public override int SaveChanges()
        {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // La lógica de auditoría se implementará en el servicio de auditoría
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}

