using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Data;
using SistemaPermisos.Models;
using SistemaPermisos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SistemaPermisos.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditService _auditService;

        public UserService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IAuditService auditService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
        }

        public async Task<IEnumerable<Usuario>> GetAllUsersAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario> GetByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<bool> GetUserByIdAsync(int id, out Usuario usuario)
        {
            usuario = await _context.Usuarios.FindAsync(id);
            return usuario != null;
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == email);
        }

        public async Task<bool> GetUserByEmailAsync(string email, out Usuario usuario)
        {
            usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == email);
            return usuario != null;
        }

        public async Task<IEnumerable<Usuario>> FindAsync(Func<Usuario, bool> predicate)
        {
            return _context.Usuarios.Where(predicate).ToList();
        }

        public async Task<bool> CreateAsync(Usuario usuario)
        {
            try
            {
                // Verificar si ya existe un usuario con el mismo correo
                if (await _context.Usuarios.AnyAsync(u => u.Correo == usuario.Correo))
                {
                    return false;
                }

                // Hashear la contraseña
                usuario.Password = HashPassword(usuario.Password);
                usuario.FechaRegistro = DateTime.Now;
                usuario.UltimaActualizacion = DateTime.Now;
                usuario.Activo = true;

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                await _auditService.LogActivityAsync(null, "Crear", "Usuario", usuario.Id, null, $"Usuario creado: {usuario.Nombre}");
                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al crear usuario: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CreateUserAsync(UserCreateViewModel model)
        {
            try
            {
                // Verificar si el correo ya existe
                if (await _context.Usuarios.AnyAsync(u => u.Correo == model.Correo))
                {
                    return false;
                }

                var usuario = new Usuario
                {
                    Nombre = model.Nombre,
                    Correo = model.Correo,
                    Password = HashPassword(model.Password),
                    Rol = model.Rol,
                    Cedula = model.Cedula,
                    Puesto = model.Puesto,
                    Telefono = model.Telefono,
                    Departamento = model.Departamento,
                    FechaRegistro = DateTime.Now,
                    UltimaActualizacion = DateTime.Now,
                    Activo = true
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // Registrar en auditoría
                await _auditService.LogActivityAsync(null, "Crear", "Usuario", usuario.Id, null, $"Nuevo usuario: {usuario.Nombre} ({usuario.Correo})");

                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al crear usuario: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Usuario usuario)
        {
            try
            {
                var existingUser = await _context.Usuarios.FindAsync(usuario.Id);
                if (existingUser == null)
                {
                    return false;
                }

                // Actualizar propiedades
                existingUser.Nombre = usuario.Nombre;
                existingUser.Telefono = usuario.Telefono;
                existingUser.Departamento = usuario.Departamento;
                existingUser.FechaNacimiento = usuario.FechaNacimiento;
                existingUser.Direccion = usuario.Direccion;
                existingUser.Cedula = usuario.Cedula;
                existingUser.Puesto = usuario.Puesto;
                existingUser.UltimaActualizacion = DateTime.Now;

                // Si se proporciona una nueva foto de perfil, actualizarla
                if (!string.IsNullOrEmpty(usuario.FotoPerfil))
                {
                    existingUser.FotoPerfil = usuario.FotoPerfil;
                }

                await _context.SaveChangesAsync();
                await _auditService.LogActivityAsync(null, "Actualizar", "Usuario", usuario.Id, null, $"Usuario actualizado: {usuario.Nombre}");
                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al actualizar usuario: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(UserEditViewModel model)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(model.Id);
                if (usuario == null)
                {
                    return false;
                }

                // Verificar si el correo ya existe en otro usuario
                if (usuario.Correo != model.Correo &&
                    await _context.Usuarios.AnyAsync(u => u.Correo == model.Correo))
                {
                    return false;
                }

                // Guardar datos antiguos para auditoría
                string datosAntiguos = $"Nombre: {usuario.Nombre}, Correo: {usuario.Correo}, Rol: {usuario.Rol}";

                usuario.Nombre = model.Nombre;
                usuario.Correo = model.Correo;
                usuario.Rol = model.Rol;
                usuario.Cedula = model.Cedula;
                usuario.Puesto = model.Puesto;
                usuario.Telefono = model.Telefono;
                usuario.Departamento = model.Departamento;
                usuario.Direccion = model.Direccion;
                usuario.UltimaActualizacion = DateTime.Now;

                // Si se proporciona una nueva contraseña, actualizarla
                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    usuario.Password = HashPassword(model.NewPassword);
                }

                await _context.SaveChangesAsync();

                // Registrar en auditoría
                string datosNuevos = $"Nombre: {usuario.Nombre}, Correo: {usuario.Correo}, Rol: {usuario.Rol}";
                await _auditService.LogActivityAsync(null, "Actualizar", "Usuario", usuario.Id, datosAntiguos, datosNuevos);

                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al actualizar usuario: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return false;
                }

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                await _auditService.LogActivityAsync(null, "Eliminar", "Usuario", id, $"Usuario eliminado: {usuario.Nombre}", null);
                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al eliminar usuario: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return false;
                }

                // Verificar si el usuario tiene registros asociados
                bool tienePermisos = await _context.Permisos.AnyAsync(p => p.UsuarioId == id);
                bool tieneOmisiones = await _context.OmisionesMarca.AnyAsync(o => o.UsuarioId == id);
                bool tieneReportes = await _context.ReportesDanos.AnyAsync(r => r.UsuarioId == id);

                if (tienePermisos || tieneOmisiones || tieneReportes)
                {
                    return false;
                }

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                // Registrar en auditoría
                await _auditService.LogActivityAsync(null, "Eliminar", "Usuario", id, $"Usuario eliminado: {usuario.Nombre} ({usuario.Correo})", null);

                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al eliminar usuario: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ChangeRoleAsync(int userId, string newRole)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(userId);
                if (usuario == null)
                {
                    return false;
                }

                string rolAnterior = usuario.Rol;
                usuario.Rol = newRole;
                usuario.UltimaActualizacion = DateTime.Now;

                await _context.SaveChangesAsync();
                await _auditService.LogActivityAsync(null, "Cambiar Rol", "Usuario", userId, $"Rol anterior: {rolAnterior}", $"Nuevo rol: {newRole}");
                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al cambiar rol: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ChangeUserRoleAsync(ChangeRoleViewModel model)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(model.UsuarioId);
                if (usuario == null)
                {
                    return false;
                }

                string rolAnterior = usuario.Rol;
                usuario.Rol = model.NuevoRol;
                usuario.UltimaActualizacion = DateTime.Now;

                await _context.SaveChangesAsync();

                // Registrar en auditoría
                await _auditService.LogActivityAsync(null, "Cambiar Rol", "Usuario", usuario.Id,
                    $"Rol anterior: {rolAnterior}", $"Nuevo rol: {usuario.Rol}");

                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al cambiar rol: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ToggleActiveStatusAsync(int userId)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(userId);
                if (usuario == null)
                {
                    return false;
                }

                usuario.Activo = !usuario.Activo;
                usuario.UltimaActualizacion = DateTime.Now;

                await _context.SaveChangesAsync();
                string action = usuario.Activo ? "Activar" : "Desactivar";
                await _auditService.LogActivityAsync(null, action, "Usuario", userId, $"Estado anterior: {!usuario.Activo}", $"Nuevo estado: {usuario.Activo}");
                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al cambiar estado activo: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ToggleUserStatusAsync(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return false;
                }

                usuario.Activo = !usuario.Activo;
                usuario.UltimaActualizacion = DateTime.Now;

                await _context.SaveChangesAsync();

                // Registrar en auditoría
                string accion = usuario.Activo ? "Activar" : "Desactivar";
                await _auditService.LogActivityAsync(null, accion, "Usuario", usuario.Id,
                    $"Estado anterior: {!usuario.Activo}", $"Nuevo estado: {usuario.Activo}");

                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al cambiar estado: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(userId);
                if (usuario == null)
                {
                    return false;
                }

                // Verificar la contraseña actual
                if (!VerifyPassword(currentPassword, usuario.Password))
                {
                    return false;
                }

                // Actualizar la contraseña
                usuario.Password = HashPassword(newPassword);
                usuario.UltimaActualizacion = DateTime.Now;

                await _context.SaveChangesAsync();
                await _auditService.LogActivityAsync(userId, "Cambiar Contraseña", "Usuario", userId, null, "Contraseña actualizada");
                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al cambiar contraseña: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ValidatePasswordAsync(int userId, string password)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(userId);
                if (usuario == null)
                {
                    return false;
                }

                return VerifyPassword(password, usuario.Password);
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al validar contraseña: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AuthenticateAsync(string email, string password)
        {
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == email);
                if (usuario == null || !usuario.Activo)
                {
                    return false;
                }

                // Verificar la contraseña
                if (!VerifyPassword(password, usuario.Password))
                {
                    return false;
                }

                // Guardar información del usuario en la sesión
                _httpContextAccessor.HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
                _httpContextAccessor.HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
                _httpContextAccessor.HttpContext.Session.SetString("UsuarioRol", usuario.Rol);
                _httpContextAccessor.HttpContext.Session.SetString("UsuarioCorreo", usuario.Correo);

                await _auditService.LogActivityAsync(usuario.Id, "Iniciar Sesión", "Usuario", usuario.Id, null, "Inicio de sesión exitoso");
                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al autenticar: {ex.Message}");
                return false;
            }
        }

        public void Logout()
        {
            try
            {
                var usuarioIdString = _httpContextAccessor.HttpContext?.Session.GetString("UsuarioId");
                int? usuarioId = null;

                if (!string.IsNullOrEmpty(usuarioIdString) && int.TryParse(usuarioIdString, out int id))
                {
                    usuarioId = id;
                }

                // Registrar la actividad antes de cerrar la sesión
                if (usuarioId.HasValue)
                {
                    _auditService.LogActivityAsync(usuarioId, "Cerrar Sesión", "Usuario", usuarioId, null, "Cierre de sesión").Wait();
                }

                // Limpiar la sesión
                _httpContextAccessor.HttpContext.Session.Clear();
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al cerrar sesión: {ex.Message}");
            }
        }

        public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(userId);
                if (usuario == null)
                {
                    return false;
                }

                // Actualizar la contraseña
                usuario.Password = HashPassword(newPassword);
                usuario.UltimaActualizacion = DateTime.Now;

                await _context.SaveChangesAsync();
                await _auditService.LogActivityAsync(userId, "Restablecer Contraseña", "Usuario", userId, null, "Contraseña restablecida");
                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al restablecer contraseña: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CreatePasswordResetTokenAsync(string email)
        {
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == email);
                if (usuario == null || !usuario.Activo)
                {
                    return false;
                }

                // Generar token
                var token = GenerateRandomToken();
                var expiration = DateTime.Now.AddHours(24);

                // Guardar token en la base de datos
                var passwordReset = new PasswordReset
                {
                    UsuarioId = usuario.Id,
                    Token = token,
                    FechaExpiracion = expiration,
                    Utilizado = false
                };

                _context.PasswordResets.Add(passwordReset);
                await _context.SaveChangesAsync();
                await _auditService.LogActivityAsync(null, "Solicitar Restablecimiento", "Usuario", usuario.Id, null, "Token de restablecimiento generado");
                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al crear token: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> InitiatePasswordResetAsync(string email)
        {
            try
            {
                var usuario = await GetByEmailAsync(email);
                if (usuario == null)
                {
                    return false;
                }

                // Generar token único
                string token = GenerateRandomToken();

                // Guardar token en la base de datos
                var passwordReset = new PasswordReset
                {
                    UsuarioId = usuario.Id,
                    Token = token,
                    FechaExpiracion = DateTime.Now.AddHours(24), // Expira en 24 horas
                    Utilizado = false
                };

                _context.PasswordResets.Add(passwordReset);
                await _context.SaveChangesAsync();

                // Registrar en auditoría
                await _auditService.LogActivityAsync(null, "Solicitar Restablecimiento", "Usuario", usuario.Id,
                    null, $"Token generado para {usuario.Correo}");

                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al iniciar restablecimiento: {ex.Message}");
                return false;
            }
        }

        public async Task<(bool isValid, int userId)> ValidatePasswordResetTokenAsync(string token)
        {
            try
            {
                var passwordReset = await _context.PasswordResets
                    .FirstOrDefaultAsync(p => p.Token == token && !p.Utilizado && p.FechaExpiracion > DateTime.Now);

                if (passwordReset == null)
                {
                    return (false, 0);
                }

                return (true, passwordReset.UsuarioId);
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al validar token: {ex.Message}");
                return (false, 0);
            }
        }

        public async Task<bool> CompletePasswordResetAsync(string token, string newPassword)
        {
            try
            {
                // Buscar el token en la base de datos
                var passwordReset = await _context.PasswordResets
                    .FirstOrDefaultAsync(pr =>
                        pr.Token == token &&
                        pr.FechaExpiracion > DateTime.Now &&
                        !pr.Utilizado);

                if (passwordReset == null)
                {
                    return false;
                }

                // Cambiar la contraseña del usuario
                var usuario = await _context.Usuarios.FindAsync(passwordReset.UsuarioId);
                if (usuario == null)
                {
                    return false;
                }

                usuario.Password = HashPassword(newPassword);
                usuario.UltimaActualizacion = DateTime.Now;

                // Marcar el token como utilizado
                passwordReset.Utilizado = true;

                await _context.SaveChangesAsync();

                // Registrar en auditoría
                await _auditService.LogActivityAsync(usuario.Id, "Restablecer Contraseña", "Usuario", usuario.Id,
                    null, "Contraseña restablecida mediante token");

                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al completar restablecimiento: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> MarkTokenAsUsedAsync(string token)
        {
            try
            {
                var passwordReset = await _context.PasswordResets
                    .FirstOrDefaultAsync(p => p.Token == token);

                if (passwordReset == null)
                {
                    return false;
                }

                passwordReset.Utilizado = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al marcar token como usado: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> HasPermissionAsync(int userId, string permission)
        {
            try
            {
                var userPermissions = await _context.UserPermissions
                    .AnyAsync(p => p.UsuarioId == userId && p.Permiso == permission);
                return userPermissions;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al verificar permiso: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<string>> GetUserPermissionsAsync(int userId)
        {
            try
            {
                var userPermissions = await _context.UserPermissions
                    .Where(p => p.UsuarioId == userId)
                    .Select(p => p.Permiso)
                    .ToListAsync();

                return userPermissions;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al obtener permisos: {ex.Message}");
                return Enumerable.Empty<string>();
            }
        }

        public async Task<bool> AddPermissionAsync(int userId, string permission)
        {
            try
            {
                // Verificar si el usuario existe
                if (!await _context.Usuarios.AnyAsync(u => u.Id == userId))
                {
                    return false;
                }

                // Verificar si ya tiene el permiso
                if (await _context.UserPermissions.AnyAsync(p => p.UsuarioId == userId && p.Permiso == permission))
                {
                    return true; // Ya tiene el permiso, no es necesario agregarlo
                }

                var userPermission = new UserPermission
                {
                    UsuarioId = userId,
                    Permiso = permission
                };

                _context.UserPermissions.Add(userPermission);
                await _context.SaveChangesAsync();

                // Registrar en auditoría
                await _auditService.LogActivityAsync(null, "Agregar Permiso", "Usuario", userId,
                    null, $"Permiso agregado: {permission}");

                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al agregar permiso: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemovePermissionAsync(int userId, string permission)
        {
            try
            {
                var userPermission = await _context.UserPermissions
                    .FirstOrDefaultAsync(p => p.UsuarioId == userId && p.Permiso == permission);

                if (userPermission == null)
                {
                    return false;
                }

                _context.UserPermissions.Remove(userPermission);
                await _context.SaveChangesAsync();

                // Registrar en auditoría
                await _auditService.LogActivityAsync(null, "Eliminar Permiso", "Usuario", userId,
                    $"Permiso eliminado: {permission}", null);

                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al eliminar permiso: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Enable2FAAsync(int userId)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(userId);
                if (usuario == null)
                {
                    return false;
                }

                // Verificar si ya tiene 2FA
                var twoFactorAuth = await _context.TwoFactorAuth
                    .FirstOrDefaultAsync(t => t.UsuarioId == userId);

                if (twoFactorAuth == null)
                {
                    // Crear nuevo registro de 2FA
                    twoFactorAuth = new TwoFactorAuth
                    {
                        UsuarioId = userId,
                        Habilitado = true,
                        ClaveSecreta = GenerateSecretKey(),
                        FechaActualizacion = DateTime.Now
                    };
                    _context.TwoFactorAuth.Add(twoFactorAuth);
                }
                else
                {
                    // Actualizar registro existente
                    twoFactorAuth.Habilitado = true;
                    twoFactorAuth.ClaveSecreta = GenerateSecretKey();
                    twoFactorAuth.FechaActualizacion = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                // Registrar en auditoría
                await _auditService.LogActivityAsync(userId, "Habilitar 2FA", "Usuario", userId,
                    null, "Autenticación de dos factores habilitada");

                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al habilitar 2FA: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Disable2FAAsync(int userId)
        {
            try
            {
                var twoFactorAuth = await _context.TwoFactorAuth
                    .FirstOrDefaultAsync(t => t.UsuarioId == userId);

                if (twoFactorAuth == null || !twoFactorAuth.Habilitado)
                {
                    return false;
                }

                twoFactorAuth.Habilitado = false;
                twoFactorAuth.FechaActualizacion = DateTime.Now;

                await _context.SaveChangesAsync();

                // Registrar en auditoría
                await _auditService.LogActivityAsync(userId, "Deshabilitar 2FA", "Usuario", userId,
                    null, "Autenticación de dos factores deshabilitada");

                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al deshabilitar 2FA: {ex.Message}");
                return false;
            }
        }

        public async Task<string> Generate2FACodeAsync(int userId)
        {
            try
            {
                var twoFactorAuth = await _context.TwoFactorAuth
                    .FirstOrDefaultAsync(t => t.UsuarioId == userId && t.Habilitado);

                if (twoFactorAuth == null)
                {
                    return null;
                }

                // Generar código de 6 dígitos
                string code = GenerateRandomCode();

                // Guardar código y establecer expiración (5 minutos)
                twoFactorAuth.UltimoCodigo = code;
                twoFactorAuth.FechaExpiracionCodigo = DateTime.Now.AddMinutes(5);

                await _context.SaveChangesAsync();

                return code;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al generar código 2FA: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> Verify2FACodeAsync(int userId, string code)
        {
            try
            {
                var twoFactorAuth = await _context.TwoFactorAuth
                    .FirstOrDefaultAsync(t =>
                        t.UsuarioId == userId &&
                        t.Habilitado &&
                        t.UltimoCodigo == code &&
                        t.FechaExpiracionCodigo > DateTime.Now);

                if (twoFactorAuth == null)
                {
                    return false;
                }

                // Invalidar el código después de usarlo
                twoFactorAuth.UltimoCodigo = null;
                twoFactorAuth.FechaExpiracionCodigo = null;

                await _context.SaveChangesAsync();

                // Registrar en auditoría
                await _auditService.LogActivityAsync(userId, "Verificar 2FA", "Usuario", userId,
                    null, "Código 2FA verificado correctamente");

                return true;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al verificar código 2FA: {ex.Message}");
                return false;
            }
        }

        #region Helper Methods
        private string HashPassword(string password)
        {
            // Usar BCrypt para hashear la contraseña
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        private string GenerateRandomToken()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var tokenData = new byte[32];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData)
                    .Replace("/", "_")
                    .Replace("+", "-")
                    .Replace("=", "");
            }
        }

        private string GenerateSecretKey()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var keyData = new byte[20];
                rng.GetBytes(keyData);
                return Convert.ToBase64String(keyData);
            }
        }

        private string GenerateRandomCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
        #endregion
    }
}
