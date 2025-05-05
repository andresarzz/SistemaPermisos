using Microsoft.EntityFrameworkCore;
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
        private readonly IEmailService _emailService;

        public UserService(IUnitOfWork unitOfWork, IAuditService auditService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _auditService = auditService;
            _emailService = emailService;
        }

        public async Task<IEnumerable<Usuario>> GetAllUsersAsync()
        {
            return await _unitOfWork.Usuarios.GetAllAsync();
        }

        public async Task<Usuario> GetUserByIdAsync(int id)
        {
            return await _unitOfWork.Usuarios.GetByIdAsync(id);
        }

        public async Task<Usuario> GetUserByEmailAsync(string email)
        {
            var users = await _unitOfWork.Usuarios.FindAsync(u => u.Correo == email);
            return users.FirstOrDefault();
        }

        public async Task<bool> CreateUserAsync(UserCreateViewModel model)
        {
            // Verificar si el correo ya existe
            if (await _unitOfWork.Usuarios.ExistsAsync(u => u.Correo == model.Correo))
            {
                return false;
            }

            var usuario = new Usuario
            {
                Nombre = model.Nombre,
                Correo = model.Correo,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Rol = model.Rol,
                Cedula = model.Cedula,
                Puesto = model.Puesto,
                Telefono = model.Telefono,
                Departamento = model.Departamento,
                FechaRegistro = DateTime.Now,
                UltimaActualizacion = DateTime.Now,
                Activo = true
            };

            await _unitOfWork.Usuarios.AddAsync(usuario);
            await _unitOfWork.CompleteAsync();

            // Registrar en auditoría
            await _auditService.LogActivityAsync(null, "Crear", "Usuarios", usuario.Id, null, $"Nuevo usuario: {usuario.Nombre} ({usuario.Correo})");

            // Enviar correo de bienvenida
            await _emailService.SendWelcomeEmailAsync(usuario.Correo, usuario.Nombre);

            return true;
        }

        public async Task<bool> UpdateUserAsync(UserEditViewModel model)
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(model.Id);
            if (usuario == null)
            {
                return false;
            }

            // Verificar si el correo ya existe en otro usuario
            if (usuario.Correo != model.Correo &&
                await _unitOfWork.Usuarios.ExistsAsync(u => u.Correo == model.Correo))
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
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            }

            await _unitOfWork.Usuarios.UpdateAsync(usuario);
            await _unitOfWork.CompleteAsync();

            // Registrar en auditoría
            string datosNuevos = $"Nombre: {usuario.Nombre}, Correo: {usuario.Correo}, Rol: {usuario.Rol}";
            await _auditService.LogActivityAsync(null, "Actualizar", "Usuarios", usuario.Id, datosAntiguos, datosNuevos);

            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
            if (usuario == null)
            {
                return false;
            }

            // Verificar si el usuario tiene registros asociados
            bool tienePermisos = await _unitOfWork.Permisos.ExistsAsync(p => p.UsuarioId == id);
            bool tieneOmisiones = await _unitOfWork.OmisionesMarca.ExistsAsync(o => o.UsuarioId == id);
            bool tieneReportes = await _unitOfWork.ReportesDanos.ExistsAsync(r => r.UsuarioId == id);

            if (tienePermisos || tieneOmisiones || tieneReportes)
            {
                return false;
            }

            await _unitOfWork.Usuarios.RemoveAsync(usuario);
            await _unitOfWork.CompleteAsync();

            // Registrar en auditoría
            await _auditService.LogActivityAsync(null, "Eliminar", "Usuarios", id, $"Usuario eliminado: {usuario.Nombre} ({usuario.Correo})", null);

            return true;
        }

        public async Task<bool> ChangeUserRoleAsync(ChangeRoleViewModel model)
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(model.UsuarioId);
            if (usuario == null)
            {
                return false;
            }

            string rolAnterior = usuario.Rol;
            usuario.Rol = model.NuevoRol;
            usuario.UltimaActualizacion = DateTime.Now;

            await _unitOfWork.Usuarios.UpdateAsync(usuario);
            await _unitOfWork.CompleteAsync();

            // Registrar en auditoría
            await _auditService.LogActivityAsync(null, "Cambiar Rol", "Usuarios", usuario.Id,
                $"Rol anterior: {rolAnterior}", $"Nuevo rol: {usuario.Rol}");

            return true;
        }

        public async Task<bool> ToggleUserStatusAsync(int id)
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
            if (usuario == null)
            {
                return false;
            }

            usuario.Activo = !usuario.Activo;
            usuario.UltimaActualizacion = DateTime.Now;

            await _unitOfWork.Usuarios.UpdateAsync(usuario);
            await _unitOfWork.CompleteAsync();

            // Registrar en auditoría
            string accion = usuario.Activo ? "Activar" : "Desactivar";
            await _auditService.LogActivityAsync(null, accion, "Usuarios", usuario.Id,
                $"Estado anterior: {!usuario.Activo}", $"Nuevo estado: {usuario.Activo}");

            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string newPassword)
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(userId);
            if (usuario == null)
            {
                return false;
            }

            usuario.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            usuario.UltimaActualizacion = DateTime.Now;

            await _unitOfWork.Usuarios.UpdateAsync(usuario);
            await _unitOfWork.CompleteAsync();

            // Registrar en auditoría
            await _auditService.LogActivityAsync(userId, "Cambiar Contraseña", "Usuarios", userId, null, null);

            return true;
        }

        public async Task<bool> ValidatePasswordAsync(int userId, string password)
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(userId);
            if (usuario == null)
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(password, usuario.Password);
        }

        public async Task<bool> HasPermissionAsync(int userId, string permission)
        {
            var permissions = await _unitOfWork.UserPermissions.FindAsync(p => p.UsuarioId == userId && p.Permiso == permission);
            return permissions.Any();
        }

        public async Task<IEnumerable<string>> GetUserPermissionsAsync(int userId)
        {
            var userPermissions = await _unitOfWork.UserPermissions
                .GetAllAsync(p => p.UsuarioId == userId);

            return userPermissions?.Select(p => p.PermissionName) ?? Enumerable.Empty<string>();
        }

        public async Task<bool> AddPermissionAsync(int userId, string permission)
        {
            // Verificar si el usuario existe
            if (!await _unitOfWork.Usuarios.ExistsAsync(u => u.Id == userId))
            {
                return false;
            }

            // Verificar si ya tiene el permiso
            if (await HasPermissionAsync(userId, permission))
            {
                return true; // Ya tiene el permiso, no es necesario agregarlo
            }

            var userPermission = new UserPermission
            {
                UsuarioId = userId,
                Permiso = permission
            };

            await _unitOfWork.UserPermissions.AddAsync(userPermission);
            await _unitOfWork.CompleteAsync();

            // Registrar en auditoría
            await _auditService.LogActivityAsync(null, "Agregar Permiso", "UserPermissions", userId,
                null, $"Permiso agregado: {permission}");

            return true;
        }

        public async Task<bool> RemovePermissionAsync(int userId, string permission)
        {
            var permissions = await _unitOfWork.UserPermissions.FindAsync(p => p.UsuarioId == userId && p.Permiso == permission);
            var userPermission = permissions.FirstOrDefault();

            if (userPermission == null)
            {
                return false;
            }

            await _unitOfWork.UserPermissions.RemoveAsync(userPermission);
            await _unitOfWork.CompleteAsync();

            // Registrar en auditoría
            await _auditService.LogActivityAsync(null, "Eliminar Permiso", "UserPermissions", userId,
                $"Permiso eliminado: {permission}", null);

            return true;
        }

        public async Task<bool> InitiatePasswordResetAsync(string email)
        {
            var usuario = await GetUserByEmailAsync(email);
            if (usuario == null)
            {
                return false;
            }

            // Generar token único
            string token = GenerateResetToken();

            // Guardar token en la base de datos
            var passwordReset = new PasswordReset
            {
                UsuarioId = usuario.Id,
                Token = token,
                FechaExpiracion = DateTime.Now.AddHours(24), // Expira en 24 horas
                Utilizado = false
            };

            await _unitOfWork.PasswordResets.AddAsync(passwordReset);
            await _unitOfWork.CompleteAsync();

            // Enviar correo con el enlace de restablecimiento
            string resetLink = $"https://yourdomain.com/Account/ResetPassword?token={token}";
            await _emailService.SendPasswordResetEmailAsync(usuario.Correo, usuario.Nombre, resetLink);

            // Registrar en auditoría
            await _auditService.LogActivityAsync(null, "Solicitar Restablecimiento", "PasswordResets", usuario.Id,
                null, $"Token generado para {usuario.Correo}");

            return true;
        }

        public async Task<bool> CompletePasswordResetAsync(string token, string newPassword)
        {
            // Buscar el token en la base de datos
            var passwordResets = await _unitOfWork.PasswordResets.FindAsync(pr =>
                pr.Token == token &&
                pr.FechaExpiracion > DateTime.Now &&
                !pr.Utilizado);

            var passwordReset = passwordResets.FirstOrDefault();
            if (passwordReset == null)
            {
                return false;
            }

            // Cambiar la contraseña del usuario
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(passwordReset.UsuarioId);
            if (usuario == null)
            {
                return false;
            }

            usuario.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            usuario.UltimaActualizacion = DateTime.Now;

            await _unitOfWork.Usuarios.UpdateAsync(usuario);

            // Marcar el token como utilizado
            passwordReset.Utilizado = true;
            await _unitOfWork.PasswordResets.UpdateAsync(passwordReset);

            await _unitOfWork.CompleteAsync();

            // Registrar en auditoría
            await _auditService.LogActivityAsync(usuario.Id, "Restablecer Contraseña", "Usuarios", usuario.Id,
                null, "Contraseña restablecida mediante token");

            return true;
        }

        public async Task<bool> Enable2FAAsync(int userId)
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(userId);
            if (usuario == null)
            {
                return false;
            }

            // Verificar si ya tiene 2FA
            var twoFactorAuth = (await _unitOfWork.TwoFactorAuth.FindAsync(t => t.UsuarioId == userId)).FirstOrDefault();

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
                await _unitOfWork.TwoFactorAuth.AddAsync(twoFactorAuth);
            }
            else
            {
                // Actualizar registro existente
                twoFactorAuth.Habilitado = true;
                twoFactorAuth.ClaveSecreta = GenerateSecretKey();
                twoFactorAuth.FechaActualizacion = DateTime.Now;
                await _unitOfWork.TwoFactorAuth.UpdateAsync(twoFactorAuth);
            }

            await _unitOfWork.CompleteAsync();

            // Registrar en auditoría
            await _auditService.LogActivityAsync(userId, "Habilitar 2FA", "TwoFactorAuth", userId,
                null, "Autenticación de dos factores habilitada");

            return true;
        }

        public async Task<bool> Disable2FAAsync(int userId)
        {
            var twoFactorAuth = (await _unitOfWork.TwoFactorAuth.FindAsync(t => t.UsuarioId == userId)).FirstOrDefault();
            if (twoFactorAuth == null || !twoFactorAuth.Habilitado)
            {
                return false;
            }

            twoFactorAuth.Habilitado = false;
            twoFactorAuth.FechaActualizacion = DateTime.Now;

            await _unitOfWork.TwoFactorAuth.UpdateAsync(twoFactorAuth);
            await _unitOfWork.CompleteAsync();

            // Registrar en auditoría
            await _auditService.LogActivityAsync(userId, "Deshabilitar 2FA", "TwoFactorAuth", userId,
                null, "Autenticación de dos factores deshabilitada");

            return true;
        }

        public async Task<string> Generate2FACodeAsync(int userId)
        {
            var twoFactorAuth = (await _unitOfWork.TwoFactorAuth.FindAsync(t => t.UsuarioId == userId && t.Habilitado)).FirstOrDefault();
            if (twoFactorAuth == null)
            {
                return null;
            }

            // Generar código de 6 dígitos
            string code = GenerateRandomCode();

            // Guardar código y establecer expiración (5 minutos)
            twoFactorAuth.UltimoCodigo = code;
            twoFactorAuth.FechaExpiracionCodigo = DateTime.Now.AddMinutes(5);

            await _unitOfWork.TwoFactorAuth.UpdateAsync(twoFactorAuth);
            await _unitOfWork.CompleteAsync();

            // Enviar código por correo o SMS (aquí solo correo)
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(userId);
            if (usuario != null)
            {
                await _emailService.Send2FACodeAsync(usuario.Correo, usuario.Nombre, code);
            }

            return code;
        }

        public async Task<bool> Verify2FACodeAsync(int userId, string code)
        {
            var twoFactorAuth = (await _unitOfWork.TwoFactorAuth.FindAsync(t =>
                t.UsuarioId == userId &&
                t.Habilitado &&
                t.UltimoCodigo == code &&
                t.FechaExpiracionCodigo > DateTime.Now)).FirstOrDefault();

            if (twoFactorAuth == null)
            {
                return false;
            }

            // Invalidar el código después de usarlo
            twoFactorAuth.UltimoCodigo = null;
            twoFactorAuth.FechaExpiracionCodigo = null;

            await _unitOfWork.TwoFactorAuth.UpdateAsync(twoFactorAuth);
            await _unitOfWork.CompleteAsync();

            // Registrar en auditoría
            await _auditService.LogActivityAsync(userId, "Verificar 2FA", "TwoFactorAuth", userId,
                null, "Código 2FA verificado correctamente");

            return true;
        }

        #region Helper Methods
        private string GenerateResetToken()
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
