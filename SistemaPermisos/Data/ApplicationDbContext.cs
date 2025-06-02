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
        public DbSet<ReporteDano> ReportesDanos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Correo).IsUnique();
            });

            modelBuilder.Entity<Permiso>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId);
            });

            modelBuilder.Entity<OmisionMarca>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId);
            });

            modelBuilder.Entity<ReporteDano>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId);
            });
        }
    }
}
