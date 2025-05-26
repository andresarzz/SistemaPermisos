using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SistemaPermisos.Models;
using SistemaPermisos.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaPermisos.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                // Verificar si ya existe un usuario administrador
                if (context.Usuarios.Any(u => u.Rol == "Admin"))
                {
                    logger.LogInformation("Usuario administrador ya existe.");
                    return;
                }

                // Crear usuario administrador por defecto
                var adminUser = new Usuario
                {
                    Nombre = "Administrador del Sistema",
                    Correo = "admin@sistema.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Rol = "Admin",
                    Activo = true,
                    FechaRegistro = DateTime.Now,
                    UltimaActualizacion = DateTime.Now
                };

                context.Usuarios.Add(adminUser);
                await context.SaveChangesAsync();

                logger.LogInformation("Usuario administrador creado exitosamente:");
                logger.LogInformation("Email: admin@sistema.com");
                logger.LogInformation("Password: Admin123!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al crear el usuario administrador.");
            }
        }
    }
}
