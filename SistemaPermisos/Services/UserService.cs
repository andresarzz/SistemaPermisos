using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Data;
using SistemaPermisos.Models;
using SistemaPermisos.Repositories;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditService _auditService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUnitOfWork unitOfWork, IAuditService auditService, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _auditService = auditService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Usuario>> GetAllUsersAsync()
        {
            return await _unitOfWork.UsuarioRepository.GetAllAsync();
        }

        public async Task<Usuario> GetByIdAsync(int id)
        {
            return await _unitOfWork.UsuarioRepository.GetByIdAsync(id);
        }

        public async Task<Usuario> GetUserByIdAsync(int id)
        {
            return await _unitOfWork.UsuarioRepository.GetByIdAsync(id);
        }

        public async Task<(bool success, Usuario usuario)> GetUserWithResultAsync(int id)
        {
            var usuario = await _unitOfWork.UsuarioRepository.GetByIdAsync(id);
            return (usuario != null, usuario);
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            var users = await _unitOfWork.UsuarioRepository.FindAsync(u => u.Correo == email);
            return users.FirstOrDefault();
        }

        public async Task<Usuario> GetUserByEmailAsync(string email)
        {
            var users = await _unitOfWork.UsuarioRepository.FindAsync(u => u.Correo == email);
            return users.FirstOrDefault();
        }

        public async Task<(bool success, Usuario usuario)> GetUserByEmailWithResultAsync(string email)
        {
            var users = await _unitOfWork.UsuarioRepository.FindAsync(u => u.Correo == email);
            var usuario = users.FirstOrDefault();
            return (usuario != null, usuario);
        }

        public async Task<IEnumerable<Usuario>> FindAsync(Func<Usuario, bool> predicate)
        {
            return await _unitOfWork.UsuarioRepository.FindAsync(predicate);
        }

        public async Task<bool> CreateAsync(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            if (string.IsNullOrEmpty(usuario.Password))
                throw new ArgumentException("La contraseña no puede estar vacía", nameof(usuario.Password));

            // Verificar si el correo ya existe
            if (await IsEmailUniqueAsync(usuario.Correo) == false)
                return false;

            // Hashear la contraseña
            usuario.Password = HashPassword(usuario.Password);
            usuario.FechaRegistro = DateTime.Now;
            usuario.UltimaActualizacion = DateTime.Now;

            await _unitOfWork.UsuarioRepository.AddAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            // Registrar en auditoría
            await _auditService.LogActionAsync("Crear", "Usuario", usuario.Id, null, Newtonsoft.Json.JsonConvert.SerializeObject(usuario));

            return true;
        }

        public async Task<bool> CreateUserAsync(UserCreateViewModel model)
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
                FechaRegistro = DateTime.Now,
                UltimaActualizacion = DateTime.Now,
                Activo = true
            };

            return await CreateUserAsync(user, model.Password);
        }

        public async Task<bool> CreateUserAsync(Usuario user, string password)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("La contraseña no puede estar vacía", nameof(password));

            // Verificar si el correo ya existe
            if (await IsEmailUniqueAsync(user.Correo) == false)
                return false;

            // Hashear la contraseña
            user.Password = HashPassword(password);
            user.FechaRegistro = DateTime.Now;
            user.UltimaActualizacion = DateTime.Now;

            await _unitOfWork.UsuarioRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Registrar en auditoría
            await _auditService.LogActionAsync("Crear", "Usuario", user.Id, null, Newtonsoft.Json.JsonConvert.SerializeObject(user));

            return true;
        }

        public async Task<bool> UpdateAsync(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            // Obtener usuario actual para comparar
            var currentUser = await _unitOfWork.UsuarioRepository.GetByIdAsync(usuario.Id);
            if (currentUser == null)
                return false;

            // Verificar si el correo ya existe (si ha cambiado)
            if (currentUser.Correo != usuario.Correo && await IsEmailUniqueAsync(usuario.Correo, usuario.Id) == false)
                return false;

            // Guardar datos antiguos para auditoría
            var oldData = Newtonsoft.Json.JsonConvert.SerializeObject(currentUser);

            // Actualizar propiedades
            currentUser.Nombre = usuario.Nombre;
            currentUser.Telefono = usuario.Telefono;
            currentUser.Departamento = usuario.Departamento;
            currentUser.FechaNacimiento = usuario.FechaNacimiento;
            currentUser.Direccion = usuario.Direccion;
            currentUser.Cedula = usuario.Cedula;
            currentUser.Puesto = usuario.Puesto;
            currentUser.UltimaActualizacion = DateTime.Now;
            currentUser.FotoPerfil = usuario.FotoPerfil;

            // No actualizar contraseña ni rol aquí

            _unitOfWork.UsuarioRepository.Update(currentUser);
            await _unitOfWork.SaveChangesAsync();

            // Registrar en auditoría
            await _auditService.LogActionAsync("Actualizar", "Usuario", usuario.Id, oldData, Newtonsoft.Json.JsonConvert.SerializeObject(currentUser));

            return true;
        }

        public async Task<bool> UpdateUserAsync(UserEditViewModel model)
        {
            var user = await _unitOfWork.UsuarioRepository.GetByIdAsync(model.Id);
            if (user == null)
                return false;

            // Verificar si el correo ya existe (si ha cambiado)
            if (user.Correo != model.Correo && await IsEmailUniqueAsync(model.Correo, user.Id) == false)
                return false;

            // Guardar datos antiguos para auditoría
            var oldData = Newtonsoft.Json.JsonConvert.SerializeObject(user);

            // Actualizar propiedades
            user.Nombre = model.Nombre;
            user.Correo = model.Correo;
            user.Cedula = model.Cedula;
            user.Puesto = model.Puesto;
            user.Telefono = model.Telefono;
            user.Departamento = model.Departamento;
            user.Direccion = model.Direccion;
            user.FechaNacimiento = model.FechaNacimiento;
            user.UltimaActualizacion = DateTime.Now;
            user.Rol = model.Rol;

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                user.Password = HashPassword(model.NewPassword);
            }

            _unitOfWork.UsuarioRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Registrar en auditoría
            await _auditService.LogActionAsync("Actualizar", "Usuario", user.Id, oldData, Newtonsoft.Json.JsonConvert.SerializeObject(user));

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _unitOfWork.UsuarioRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            // Guardar datos para auditoría
            var oldData = Newtonsoft.Json.JsonConvert.SerializeObject(user);

            _unitOfWork.UsuarioRepository.Remove(user);
            await _unitOfWork.SaveChangesAsync();

            // Registrar en auditoría
            await _auditService.LogActionAsync("Eliminar", "Usuario", id, oldData, null);

            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.UsuarioRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            // Guardar datos para auditoría
            var oldData = Newtonsoft.Json.JsonConvert.SerializeObject(user);

            _unitOfWork.UsuarioRepository.Remove(user);
            await _unitOfWork.SaveChangesAsync();

            // Registrar en auditoría
            await _auditService.LogActionAsync("Eliminar", "Usuario", id, oldData, null);

            return true;
        }

        public async Task<bool> ChangeRoleAsync(int userId, string newRole)
        {
            var user = await _unitOfWork.UsuarioRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Guardar datos antiguos para auditoría
            var oldData = Newtonsoft.Json.JsonConvert.SerializeObject(user);

            // Actualizar rol
            user.Rol = newRole;
            user.UltimaActualizacion = DateTime.Now;

            _unitOfWork.UsuarioRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Registrar en auditoría
            await _auditService.LogActionAsync("Cambiar Rol", "Usuario", userId, oldData, Newtonsoft.Json.JsonConvert.SerializeObject(user));

            return true;
        }

        public async Task<bool> ChangeUserRoleAsync(ChangeRoleViewModel model)
        {
            var user = await _unitOfWork.UsuarioRepository.GetByIdAsync(model.UsuarioId);
            if (user == null)
                return false;

            // Guardar datos antiguos para auditoría
            var oldData = Newtonsoft.Json.JsonConvert.SerializeObject(user);

            // Actualizar rol
            user.Rol = model.NuevoRol;
            user.UltimaActualizacion = DateTime.Now;

            _unitOfWork.UsuarioRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Registrar en auditoría
            await _auditService.LogActionAsync("Cambiar Rol", "Usuario", user.Id, oldData, Newtonsoft.Json.JsonConvert.SerializeObject(user));

            return true;
        }

        public async Task<bool> ToggleActiveStatusAsync(int userId)
        {
            var user = await _unitOfWork.UsuarioRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Guardar datos antiguos para auditoría
            var oldData = Newtonsoft.Json.JsonConvert.SerializeObject(user);

            // Activar/Desactivar usuario
            user.Activo = !user.Activo;
            user.UltimaActualizacion = DateTime.Now;

            _unitOfWork.UsuarioRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Registrar en auditoría
            await _auditService.LogActionAsync(user.Activo ? "Activar" : "Desactivar", "Usuario", userId, oldData, Newtonsoft.Json.JsonConvert.SerializeObject(user));

            return true;
        }

        public async Task<bool> ToggleUserStatusAsync(int id)
        {
            var user = await _unitOfWork.UsuarioRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            // Guardar datos antiguos para auditoría
            var oldData = Newtonsoft.Json.JsonConvert.SerializeObject(user);

            // Activar/Desactivar usuario
            user.Activo = !user.Activo;
            user.UltimaActualizacion = DateTime.Now;

            _unitOfWork.UsuarioRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Registrar en auditoría
            await _auditService.LogActionAsync(user.Activo ? "Activar" : "Desactivar", "Usuario", id, oldData, Newtonsoft.Json.JsonConvert.SerializeObject(user));

            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _unitOfWork.UsuarioRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Verificar contraseña actual
            if (!VerifyPassword(currentPassword, user.Password))
                return false;

            // Actualizar contraseña
            user.Password = HashPassword(newPassword);
            user.UltimaActualizacion = DateTime.Now;

            _unitOfWork.UsuarioRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Registrar en auditoría (sin incluir contraseñas en el log)
            await _auditService.LogActionAsync("Cambiar Contraseña", "Usuario", userId, null, null);

            return true;
        }

        public async Task<bool> ValidatePasswordAsync(int userId, string password)
        {
            var user = await _unitOfWork.UsuarioRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            return VerifyPassword(password, user.Password);
        }

        public async Task<bool> ValidatePasswordAsync(string email, string password)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null)
                return false;

            return VerifyPassword(password, user.Password);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? userId = null)
        {
            var users = await _unitOfWork.UsuarioRepository.FindAsync(u => u.Correo == email);
            var existingUser = users.FirstOrDefault();

            // Si no existe usuario con ese correo, es único
            if (existingUser == null)
                return true;

            // Si existe pero es el mismo usuario que estamos editando, es único
            if (userId.HasValue && existingUser.Id == userId.Value)
                return true;

            // En otro caso, no es único
            return false;
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
            var user = await _unitOfWork.UsuarioRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Actualizar contraseña
            user.Password = HashPassword(newPassword);
            user.UltimaActualizacion = DateTime.Now;

            _unitOfWork.UsuarioRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Registrar en auditoría
            await _auditService.LogActionAsync("Restablecer Contraseña", "Usuario", user.Id, null, null);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null)
                return false;

            // Buscar token válido
            var passwordResets = await _unitOfWork.PasswordResetRepository.FindAsync(
                pr => pr.UsuarioId == user.Id &&
                      pr.Token == token &&
                      pr.FechaExpiracion > DateTime.Now &&
                      !pr.Utilizado);

            var passwordReset = passwordResets.FirstOrDefault();
            if (passwordReset == null)
                return false;

            // Actualizar contraseña
            user.Password = HashPassword(newPassword);
            user.UltimaActualizacion = DateTime.Now;

            // Marcar token como utilizado
            passwordReset.Utilizado = true;
            passwordReset.FechaUtilizacion = DateTime.Now;

            _unitOfWork.UsuarioRepository.Update(user);
            _unitOfWork.PasswordResetRepository.Update(passwordReset);
            await _unitOfWork.SaveChangesAsync();

            // Registrar en auditoría
            await _auditService.LogActionAsync("Restablecer Contraseña", "Usuario", user.Id, null, null);

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

            await _unitOfWork.PasswordResetRepository.AddAsync(passwordReset);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null)
                return null;

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

            await _unitOfWork.PasswordResetRepository.AddAsync(passwordReset);
            await _unitOfWork.SaveChangesAsync();

            return token;
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

            _unitOfWork.PasswordResetRepository.AddAsync(passwordReset);
            await _unitOfWork.SaveChangesAsync();

            // Registrar en auditoría
            await _auditService.LogActivityAsync(null, "Solicitar Restablecimiento", "Usuario", user.Id,
                null, $"Token generado para {user.Correo}");

            return true;
        }

        public async Task<(bool isValid, int userId)> ValidatePasswordResetTokenAsync(string token)
        {
            try
            {
                var passwordReset = await _unitOfWork.PasswordResetRepository.FindAsync(p => p.Token == token && !p.Utilizado && p.FechaExpiracion > DateTime.Now);

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
                var passwordReset = await _unitOfWork.PasswordResetRepository.FindAsync(pr =>
                        pr.Token == token &&
                        pr.FechaExpiracion > DateTime.Now &&
                        !pr.Utilizado);

                var passwordResetFirst = passwordReset.FirstOrDefault();

                if (passwordResetFirst == null)
                {
                    return false;
                }

                // Cambiar la contraseña del usuario
                var usuario = await _unitOfWork.UsuarioRepository.GetByIdAsync(passwordResetFirst.UsuarioId);
                if (usuario == null)
                {
                    return false;
                }

                usuario.Password = HashPassword(newPassword);
                usuario.UltimaActualizacion = DateTime.Now;

                // Marcar el token como utilizado
                passwordResetFirst.Utilizado = true;

                _unitOfWork.UsuarioRepository.Update(usuario);
                _unitOfWork.PasswordResetRepository.Update(passwordResetFirst);

                await _unitOfWork.SaveChangesAsync();

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
                var passwordReset = await _unitOfWork.PasswordResetRepository.FindAsync(p => p.Token == token);
                var passwordResetFirst = passwordReset.FirstOrDefault();

                if (passwordResetFirst == null)
                {
                    return false;
                }

                passwordResetFirst.Utilizado = true;
                _unitOfWork.PasswordResetRepository.Update(passwordResetFirst);
                await _unitOfWork.SaveChangesAsync();
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
                var userPermissions = await _unitOfWork.UserPermissionRepository.FindAsync(p => p.UsuarioId == userId && p.PermisoId.ToString() == permission);
                return userPermissions.Any();
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
                var userPermissions = await _unitOfWork.UserPermissionRepository.FindAsync(p => p.UsuarioId == userId);

                return userPermissions.Select(p => p.PermisoId.ToString()).ToList();
            }
            catch (Exception ex)
            {
                // Registrar la excepción
                System.Diagnostics.Debug.WriteLine($"Error al obtener permisos: {ex.Message}");
                return Enumerable.Empty<string>();
            }
        }

        public async Task<IEnumerable<UserPermission>> GetUserPermissionsAsync(int userId)
        {
            return await _unitOfWork.UserPermissionRepository.FindAsync(p => p.UsuarioId == userId);
        }

        public async Task<bool> AddPermissionAsync(int userId, string permission)
        {
            try
            {
                // Verificar si el usuario existe
                var user = await _unitOfWork.UsuarioRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                // Verificar si ya tiene el permiso
                var existingPermissions = await _unitOfWork.UserPermissionRepository.FindAsync(p => p.UsuarioId == userId && p.PermisoId.ToString() == permission);
                if (existingPermissions.Any())
                {
                    return true; // Ya tiene el permiso, no es necesario agregarlo
                }

                var userPermission = new UserPermission
                {
                    UsuarioId = userId,
                    PermisoId = int.Parse(permission)
                };

                await _unitOfWork.UserPermissionRepository.AddAsync(userPermission);
                await _unitOfWork.SaveChangesAsync();

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
                var userPermissions = await _unitOfWork.UserPermissionRepository.FindAsync(p => p.UsuarioId == userId && p.PermisoId.ToString() == permission);
                var userPermission = userPermissions.FirstOrDefault();

                if (userPermission == null)
                {
                    return false;
                }

                _unitOfWork.UserPermissionRepository.Remove(userPermission);
                await _unitOfWork.SaveChangesAsync();

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
                var usuario = await _unitOfWork.UsuarioRepository.GetByIdAsync(userId);
                if (usuario == null)
                {
                    return false;
                }

                // Verificar si ya tiene 2FA
                var twoFactorAuth = await _unitOfWork.TwoFactorAuthRepository.FindAsync(t => t.UsuarioId == userId);
                var twoFactorAuthFirst = twoFactorAuth.FirstOrDefault();

                if (twoFactorAuthFirst == null)
                {
                    // Crear nuevo registro de 2FA
                    twoFactorAuthFirst = new TwoFactorAuth
                    {
                        UsuarioId = userId,
                        Habilitado = true,
                        ClaveSecreta = GenerateSecretKey(),
                        FechaActualizacion = DateTime.Now
                    };
                    await _unitOfWork.TwoFactorAuthRepository.AddAsync(twoFactorAuthFirst);
                }
                else
                {
                    // Actualizar registro existente
                    twoFactorAuthFirst.Habilitado = true;
                    twoFactorAuthFirst.ClaveSecreta = GenerateSecretKey();
                    twoFactorAuthFirst.FechaActualizacion = DateTime.Now;
                    _unitOfWork.TwoFactorAuthRepository.Update(twoFactorAuthFirst);
                }

                await _unitOfWork.SaveChangesAsync();

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
                var twoFactorAuth = await _unitOfWork.TwoFactorAuthRepository.FindAsync(t => t.UsuarioId == userId);
                var twoFactorAuthFirst = twoFactorAuth.FirstOrDefault();

                if (twoFactorAuthFirst == null || !twoFactorAuthFirst.Habilitado)
                {
                    return false;
                }

                twoFactorAuthFirst.Habilitado = false;
                twoFactorAuthFirst.FechaActualizacion = DateTime.Now;

                _unitOfWork.TwoFactorAuthRepository.Update(twoFactorAuthFirst);
                await _unitOfWork.SaveChangesAsync();

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
                var twoFactorAuth = await _unitOfWork.TwoFactorAuthRepository.FindAsync(t => t.UsuarioId == userId && t.Habilitado);
                var twoFactorAuthFirst = twoFactorAuth.FirstOrDefault();

                if (twoFactorAuthFirst == null)
                {
                    return null;
                }

                // Generar código de 6 dígitos
                string code = GenerateRandomCode();

                // Guardar código y establecer expiración (5 minutos)
                twoFactorAuthFirst.UltimoCodigo = code;
                twoFactorAuthFirst.FechaExpiracionCodigo = DateTime.Now.AddMinutes(5);

                _unitOfWork.TwoFactorAuthRepository.Update(twoFactorAuthFirst);
                await _unitOfWork.SaveChangesAsync();

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
                var twoFactorAuth = await _unitOfWork.TwoFactorAuthRepository.FindAsync(t =>
                        t.UsuarioId == userId &&
                        t.Habilitado &&
                        t.UltimoCodigo == code &&
                        t.FechaExpiracionCodigo > DateTime.Now);

                var twoFactorAuthFirst = twoFactorAuth.FirstOrDefault();

                if (twoFactorAuthFirst == null)
                {
                    return false;
                }

                // Invalidar el código después de usarlo
                twoFactorAuthFirst.UltimoCodigo = null;
                twoFactorAuthFirst.FechaExpiracionCodigo = null;

                _unitOfWork.TwoFactorAuthRepository.Update(twoFactorAuthFirst);
                await _unitOfWork.SaveChangesAsync();

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

        public async Task<bool> DeactivateUserAsync(int id)
        {
            var user = await _unitOfWork.UsuarioRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            // Guardar datos antiguos para auditoría
            var oldData = Newtonsoft.Json.JsonConvert.SerializeObject(user);

            // Desactivar usuario
            user.Activo = false;
            user.UltimaActualizacion = DateTime.Now;

            _unitOfWork.UsuarioRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Registrar en auditoría
            await _auditService.LogActionAsync("Desactivar", "Usuario", id, oldData, Newtonsoft.Json.JsonConvert.SerializeObject(user));

            return true;
        }

        public async Task<bool> ActivateUserAsync(int id)
        {
            var user = await _unitOfWork.UsuarioRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            // Guardar datos antiguos para auditoría
            var oldData = Newtonsoft.Json.JsonConvert.SerializeObject(user);

            // Activar usuario
            user.Activo = true;
            user.UltimaActualizacion = DateTime.Now;

            _unitOfWork.UsuarioRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Registrar en auditoría
            await _auditService.LogActionAsync("Activar", "Usuario", id, oldData, Newtonsoft.Json.JsonConvert.SerializeObject(user));

            return true;
        }

        public async Task<string> GetUserFotoPerfilAsync(int id)
        {
            var user = await _unitOfWork.UsuarioRepository.GetByIdAsync(id);
            return user?.FotoPerfil;
        }

        public async Task<bool> UpdateUserFotoPerfilAsync(int id, string fotoPerfil)
        {
            var user = await _unitOfWork.UsuarioRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            // Guardar datos antiguos para auditoría
            var oldData = Newtonsoft.Json.JsonConvert.SerializeObject(user);

            // Actualizar foto de perfil
            user.FotoPerfil = fotoPerfil;
            user.UltimaActualizacion = DateTime.Now;

            _unitOfWork.UsuarioRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Registrar en auditoría
            await _auditService.LogActionAsync("Actualizar Foto de Perfil", "Usuario", id, oldData, Newtonsoft.Json.JsonConvert.SerializeObject(user));

            return true;
        }

        #region Helper Methods

        private string HashPassword(string password)
        {
            // Usar BCrypt para hashear contraseñas
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
