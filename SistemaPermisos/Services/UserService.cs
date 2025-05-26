using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Data;
using SistemaPermisos.Models;
using SistemaPermisos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SistemaPermisos.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(ApplicationDbContext context, IAuditService auditService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Usuario>> GetAllUsersAsync()
        {
            return await _context.Usuarios
                .Include(u => u.UserPermissions)
                .ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.UserPermissions)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> GetUserByIdAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.UserPermissions)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.Usuarios
                .Include(u => u.UserPermissions)
                .FirstOrDefaultAsync(u => u.Correo.ToLower() == email.ToLower());
        }

        public async Task<Usuario?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.Usuarios
                .Include(u => u.UserPermissions)
                .FirstOrDefaultAsync(u => u.Correo.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<Usuario>> FindAsync(Func<Usuario, bool> predicate)
        {
            var users = await _context.Usuarios
                .Include(u => u.UserPermissions)
                .ToListAsync();

            return users.Where(predicate);
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            // Verificar si el correo ya existe
            var existingUser = await GetUserByEmailAsync(usuario.Correo);
            if (existingUser != null)
                throw new InvalidOperationException("Ya existe un usuario con este correo electrónico");

            // Encriptar la contraseña
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
            usuario.FechaRegistro = DateTime.Now;
            usuario.UltimaActualizacion = DateTime.Now;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync("CREATE", "Usuarios", usuario.Id, null,
                $"Usuario creado: {usuario.Nombre} ({usuario.Correo})");

            return usuario;
        }

        public async Task<bool> CreateUserAsync(UserCreateViewModel model)
        {
            try
            {
                var user = new Usuario
                {
                    Nombre = model.Nombre,
                    Correo = model.Correo,
                    Rol = model.Rol,
                    Cedula = model.Cedula,
                    Puesto = model.Puesto,
                    Telefono = model.Telefono,
                    Departamento = model.Departamento,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    FechaRegistro = DateTime.Now,
                    UltimaActualizacion = DateTime.Now,
                    Activo = true
                };

                _context.Usuarios.Add(user);
                await _context.SaveChangesAsync();

                await _auditService.LogAsync("CREATE", "Usuarios", user.Id, null,
                    $"Usuario creado: {user.Nombre} ({user.Correo})");

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Usuario> UpdateAsync(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            var existingUser = await GetUserByIdAsync(usuario.Id);
            if (existingUser == null)
                throw new InvalidOperationException("Usuario no encontrado");

            var oldData = $"Nombre: {existingUser.Nombre}, Correo: {existingUser.Correo}, Rol: {existingUser.Rol}";

            existingUser.Nombre = usuario.Nombre;
            existingUser.Correo = usuario.Correo;
            existingUser.Rol = usuario.Rol;
            existingUser.Cedula = usuario.Cedula;
            existingUser.Puesto = usuario.Puesto;
            existingUser.Telefono = usuario.Telefono;
            existingUser.Departamento = usuario.Departamento;
            existingUser.FechaNacimiento = usuario.FechaNacimiento;
            existingUser.Direccion = usuario.Direccion;
            existingUser.FotoPerfil = usuario.FotoPerfil;
            existingUser.Activo = usuario.Activo;
            existingUser.UltimaActualizacion = DateTime.Now;

            _context.Usuarios.Update(existingUser);
            await _context.SaveChangesAsync();

            var newData = $"Nombre: {existingUser.Nombre}, Correo: {existingUser.Correo}, Rol: {existingUser.Rol}";
            await _auditService.LogAsync("UPDATE", "Usuarios", existingUser.Id, oldData, newData);

            return existingUser;
        }

        public async Task<bool> UpdateUserAsync(UserEditViewModel model)
        {
            try
            {
                var user = await _context.Usuarios.FindAsync(model.Id);
                if (user == null)
                    return false;

                var oldData = $"Nombre: {user.Nombre}, Correo: {user.Correo}, Rol: {user.Rol}";

                user.Nombre = model.Nombre;
                user.Correo = model.Correo;
                user.Cedula = model.Cedula;
                user.Puesto = model.Puesto;
                user.Telefono = model.Telefono;
                user.Departamento = model.Departamento;
                user.Direccion = model.Direccion;
                user.Rol = model.Rol;
                user.Activo = model.Activo;
                user.UltimaActualizacion = DateTime.Now;

                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                }

                _context.Usuarios.Update(user);
                await _context.SaveChangesAsync();

                var newData = $"Nombre: {user.Nombre}, Correo: {user.Correo}, Rol: {user.Rol}";
                await _auditService.LogAsync("UPDATE", "Usuarios", user.Id, oldData, newData);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Usuarios.FindAsync(id);
            if (user == null)
                return false;

            var userData = $"Usuario eliminado: {user.Nombre} ({user.Correo})";

            _context.Usuarios.Remove(user);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync("DELETE", "Usuarios", id, userData, null);

            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await DeleteAsync(id);
        }

        public async Task<bool> ChangeRoleAsync(int userId, string newRole)
        {
            if (string.IsNullOrWhiteSpace(newRole))
                return false;

            var usuario = await GetUserByIdAsync(userId);
            if (usuario == null)
                return false;

            var oldRole = usuario.Rol;
            usuario.Rol = newRole;
            usuario.UltimaActualizacion = DateTime.Now;

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync("UPDATE", "Usuarios", userId,
                $"Rol anterior: {oldRole}", $"Nuevo rol: {newRole}");

            return true;
        }

        public async Task<bool> ChangeUserRoleAsync(int userId, string newRole)
        {
            return await ChangeRoleAsync(userId, newRole);
        }

        public async Task<bool> ChangeUserRoleAsync(ChangeRoleViewModel model)
        {
            return await ChangeRoleAsync(model.UsuarioId, model.NuevoRol);
        }

        public async Task<bool> ValidatePasswordAsync(int userId, string password)
        {
            var user = await _context.Usuarios.FindAsync(userId);
            if (user == null)
                return false;

            return BCrypt.Net.BCrypt.Verify(password, user.Password);
        }

        public async Task<bool> ValidateUserAsync(string email, string password)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null)
                return false;

            return BCrypt.Net.BCrypt.Verify(password, user.Password);
        }

        public async Task<bool> AuthenticateAsync(string email, string password)
        {
            try
            {
                var usuario = await GetUserByEmailAsync(email);
                if (usuario == null || !usuario.Activo)
                {
                    return false;
                }

                // Verificar la contraseña
                if (!BCrypt.Net.BCrypt.Verify(password, usuario.Password))
                {
                    return false;
                }

                // Guardar información del usuario en la sesión
                _httpContextAccessor.HttpContext?.Session.SetString("UsuarioId", usuario.Id.ToString());
                _httpContextAccessor.HttpContext?.Session.SetString("UsuarioNombre", usuario.Nombre);
                _httpContextAccessor.HttpContext?.Session.SetString("UsuarioRol", usuario.Rol);
                _httpContextAccessor.HttpContext?.Session.SetString("UsuarioCorreo", usuario.Correo);

                await _auditService.LogAsync("LOGIN", "Usuarios", usuario.Id, null, "Inicio de sesión exitoso");
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
                    _auditService.LogAsync("LOGOUT", "Usuarios", usuarioId.Value, null, "Cierre de sesión").Wait();
                }

                // Limpiar la sesión
                _httpContextAccessor.HttpContext?.Session.Clear();
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al cerrar sesión: {ex.Message}");
            }
        }

        public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
        {
            var user = await _context.Usuarios.FindAsync(userId);
            if (user == null)
                return false;

            // Encriptar la contraseña
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UltimaActualizacion = DateTime.Now;

            _context.Usuarios.Update(user);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync("UPDATE", "Usuarios", user.Id, null,
                "Contraseña restablecida");

            return true;
        }

        public async Task<bool> CreatePasswordResetTokenAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null)
                return false;

            // Generar token único
            var token = Guid.NewGuid().ToString("N");

            // Guardar token en la base de datos
            var passwordReset = new PasswordReset
            {
                UsuarioId = user.Id,
                Token = token,
                FechaExpiracion = DateTime.Now.AddHours(24), // Expira en 24 horas
                Utilizado = false
            };

            _context.PasswordResets.Add(passwordReset);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> InitiatePasswordResetAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            // Generar token único
            string token = GenerateRandomToken();

            // Guardar token en la base de datos
            var passwordReset = new PasswordReset
            {
                UsuarioId = user.Id,
                Token = token,
                FechaExpiracion = DateTime.Now.AddHours(24), // Expira en 24 horas
                Utilizado = false
            };

            _context.PasswordResets.Add(passwordReset);
            await _context.SaveChangesAsync();

            // Registrar en auditoría
            await _auditService.LogAsync("CREATE", "PasswordResets", user.Id, null,
                $"Token generado para {user.Correo}");

            return true;
        }

        public async Task<(bool isValid, int userId)> ValidatePasswordResetTokenAsync(string token)
        {
            try
            {
                var passwordReset = await _context.PasswordResets
                    .FirstOrDefaultAsync(pr => pr.Token == token && !pr.Utilizado && pr.FechaExpiracion > DateTime.Now);

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

                usuario.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                usuario.UltimaActualizacion = DateTime.Now;

                // Marcar el token como utilizado
                passwordReset.Utilizado = true;

                _context.Usuarios.Update(usuario);
                _context.PasswordResets.Update(passwordReset);

                await _context.SaveChangesAsync();

                // Registrar en auditoría
                await _auditService.LogAsync("UPDATE", "Usuarios", usuario.Id, null,
                    "Contraseña restablecida mediante token");

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
                _context.PasswordResets.Update(passwordReset);
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
                    .Where(p => p.UsuarioId == userId && p.Permiso == permission && p.Activo)
                    .AnyAsync();
                return userPermissions;
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al verificar permiso: {ex.Message}");
                return false;
            }
        }

        public async Task<List<string>> GetUserPermissionsAsync(int userId)
        {
            var userPermissions = await _context.UserPermissions
                .Where(up => up.UsuarioId == userId && up.Activo)
                .Select(up => up.Permiso)
                .ToListAsync();

            return userPermissions;
        }

        public async Task<bool> UpdateUserPermissionsAsync(int userId, List<string> permissions)
        {
            if (permissions == null)
                permissions = new List<string>();

            var usuario = await GetUserByIdAsync(userId);
            if (usuario == null)
                return false;

            // Obtener permisos actuales
            var currentPermissions = await _context.UserPermissions
                .Where(up => up.UsuarioId == userId)
                .ToListAsync();

            // Eliminar permisos actuales
            _context.UserPermissions.RemoveRange(currentPermissions);

            // Agregar nuevos permisos
            foreach (var permission in permissions)
            {
                var userPermission = new UserPermission
                {
                    UsuarioId = userId,
                    Permiso = permission,
                    FechaAsignacion = DateTime.Now,
                    Activo = true
                };
                _context.UserPermissions.Add(userPermission);
            }

            await _context.SaveChangesAsync();

            await _auditService.LogAsync("UPDATE", "UserPermissions", userId,
                $"Permisos anteriores: {string.Join(", ", currentPermissions.Select(cp => cp.Permiso))}",
                $"Nuevos permisos: {string.Join(", ", permissions)}");

            return true;
        }

        public async Task<bool> UserHasPermissionAsync(int userId, string permission)
        {
            if (string.IsNullOrWhiteSpace(permission))
                return false;

            return await _context.UserPermissions
                .AnyAsync(up => up.UsuarioId == userId && up.Permiso == permission && up.Activo);
        }

        public async Task<bool> AddPermissionAsync(int userId, string permission)
        {
            try
            {
                var existingPermission = await _context.UserPermissions
                    .FirstOrDefaultAsync(up => up.UsuarioId == userId && up.Permiso == permission);

                if (existingPermission != null)
                {
                    existingPermission.Activo = true;
                    _context.UserPermissions.Update(existingPermission);
                }
                else
                {
                    var userPermission = new UserPermission
                    {
                        UsuarioId = userId,
                        Permiso = permission,
                        FechaAsignacion = DateTime.Now,
                        Activo = true
                    };
                    _context.UserPermissions.Add(userPermission);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemovePermissionAsync(int userId, string permission)
        {
            try
            {
                var userPermission = await _context.UserPermissions
                    .FirstOrDefaultAsync(up => up.UsuarioId == userId && up.Permiso == permission);

                if (userPermission != null)
                {
                    userPermission.Activo = false;
                    _context.UserPermissions.Update(userPermission);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ToggleUserStatusAsync(int userId)
        {
            try
            {
                var user = await _context.Usuarios.FindAsync(userId);
                if (user == null)
                    return false;

                user.Activo = !user.Activo;
                user.UltimaActualizacion = DateTime.Now;

                _context.Usuarios.Update(user);
                await _context.SaveChangesAsync();

                await _auditService.LogAsync("UPDATE", "Usuarios", userId,
                    $"Estado anterior: {!user.Activo}", $"Nuevo estado: {user.Activo}");

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<PaginatedList<Usuario>> GetPaginatedUsersAsync(int pageIndex, int pageSize, string? searchString = null, string? roleFilter = null)
        {
            var query = _context.Usuarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(u => u.Nombre.Contains(searchString) || u.Correo.Contains(searchString));
            }

            if (!string.IsNullOrWhiteSpace(roleFilter))
            {
                query = query.Where(u => u.Rol == roleFilter);
            }

            query = query.OrderBy(u => u.Nombre);

            return await PaginatedList<Usuario>.CreateAsync(query, pageIndex, pageSize);
        }

        #region Helper Methods

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

        #endregion
    }
}
