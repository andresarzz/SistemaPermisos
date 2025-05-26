using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SistemaPermisos.Data;
using SistemaPermisos.Middleware;
using SistemaPermisos.Services;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Configurar servicios
builder.Services.AddControllersWithViews();

// Configurar la conexión a la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// IMPORTANTE: Registrar IHttpContextAccessor antes de los servicios que lo utilizan
builder.Services.AddHttpContextAccessor();

// Registrar servicios
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IExportService, ExportService>();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Middleware personalizado para manejo de excepciones globales
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Asegurar que la base de datos esté creada y las migraciones aplicadas
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Aplicar migraciones automáticamente
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al migrar la base de datos.");
    }
}

// Usar sesión antes de la autorización
app.UseSession();

// Middleware personalizado para verificar la autenticación
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value.ToLower();
    var isAuthenticated = context.Session.GetInt32("UsuarioId") != null;

    // Rutas que no requieren autenticación
    var publicPaths = new[] {
        "/account/login",
        "/account/forgotpassword",
        "/account/resetpassword",
        "/home/error",
        "/account/accessdenied"
    };

    // Verificar si la ruta actual es pública o si contiene archivos estáticos
    var isPublicPath = Array.Exists(publicPaths, p => path.StartsWith(p)) ||
                       path.StartsWith("/lib/") ||
                       path.StartsWith("/css/") ||
                       path.StartsWith("/js/") ||
                       path.StartsWith("/images/");

    if (!isAuthenticated && !isPublicPath)
    {
        context.Response.Redirect("/Account/Login");
        return;
    }

    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
