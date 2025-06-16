using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Models;
using BCrypt.Net;

namespace SistemaPermisos.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            // Verificar si ya hay datos
            if (context.Set<Usuario>().Any())
            {
                return; // Ya hay datos
            }

            // Crear usuario administrador por defecto
            var adminUser = new Usuario
            {
                Nombre = "Administrador",
                Apellidos = "Sistema",
                Correo = "admin@sistema.com",
                NombreUsuario = "admin",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Rol = "Admin",
                Activo = true,
                FechaCreacion = DateTime.Now,
                FechaRegistro = DateTime.Now,
                UltimaActualizacion = DateTime.Now
            };

            context.Set<Usuario>().Add(adminUser);
            context.SaveChanges();
        }
    }
}
