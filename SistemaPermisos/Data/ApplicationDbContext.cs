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

            // Datos iniciales (opcional)
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nombre = "Administrador",
                    Correo = "admin@escuela.edu",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Rol = "Admin"
                },
                new Usuario
                {
                    Id = 2,
                    Nombre = "Docente Demo",
                    Correo = "docente@escuela.edu",
                    Password = BCrypt.Net.BCrypt.HashPassword("Docente123!"),
                    Rol = "Docente"
                }
            );
        }
    }
}

