using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaPermisos.Data;
using SistemaPermisos.Models;
using SistemaPermisos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaPermisos.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<Usuario> _passwordHasher;
        private readonly IAuditService _auditService;
        private readonly IEmailService _emailService;

        public UserService(ApplicationDbContext context, IAuditService auditService, IEmailService emailService)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Usuario>();
            _auditService = auditService;
            _emailService = emailService;
        }

        public async Task<Usuario?> GetUserById(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> GetUserByEmail(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario?> GetUserByUsername(string username)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == username);
        }

        public async Task<PaginatedList<Usuario>> GetPaginatedUsers(int pageNumber, int pageSize, string? searchString, string? currentFilter)
        {
            var users = from u in _context.Usuarios select u;

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.Nombre.Contains(searchString) ||
                                         u.Apellidos!.Contains(searchString) ||
                                         u.Email.Contains(searchString) ||
                                         u.NombreUsuario.Contains(searchString) ||
                                         u.Rol.Contains(searchString));
            }

            users = users.OrderBy(u => u.Nombre);

            return await PaginatedList<Usuario>.CreateAsync(users.AsNoTracking(), pageNumber, pageSize);
        }

        public async Task<ServiceResult> CreateUser(UserCreateViewModel model)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == model.Email))
            {
                return ServiceResult.ErrorResult("El correo electrónico ya está registrado.");
            }
            if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == model.NombreUsuario))
            {
                return ServiceResult.ErrorResult("El nombre de usuario ya está en uso.");
            }

            var user = new Usuario
            {
                Nombre = model.Nombre,
                Apellidos = model.Apellidos,
                NombreUsuario = model.NombreUsuario,
                Email = model.Email,
                Rol = model.Rol,
                Cedula = model.Cedula,
                Puesto = model.Puesto,
                Telefono = model.Telefono,
                Departamento = model.Departamento,
                Direccion = model.Direccion,
                FechaNacimiento = model.FechaNacimiento,
                IsActive = model.Activo,
                FechaRegistro = DateTime.Now,
                UltimaActualizacion = DateTime.Now,
                EmailConfirmed = false // Email needs to be confirmed
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            await _auditService.LogAction(user.Id, "Creación de Usuario", $"Usuario {user.NombreUsuario} creado por un administrador.");
            return ServiceResult.SuccessResult("Usuario creado exitosamente.");
        }

        public async Task<ServiceResult> UpdateUser(UserEditViewModel model)
        {
            var user = await _context.Usuarios.FindAsync(model.Id);
            if (user == null)
            {
                return ServiceResult.ErrorResult("Usuario no encontrado.");
            }

            // Check if email or username is being changed to an existing one by another user
            if (await _context.Usuarios.AnyAsync(u => u.Email == model.Email && u.Id != model.Id))
            {
                return ServiceResult.ErrorResult("El correo electrónico ya está registrado por otro usuario.");
            }
            if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == model.NombreUsuario && u.Id != model.Id))
            {
                return ServiceResult.ErrorResult("El nombre de usuario ya está en uso por otro usuario.");
            }

            user.Nombre = model.Nombre;
            user.Apellidos = model.Apellidos;
            user.NombreUsuario = model.NombreUsuario;
            user.Email = model.Email;
            user.Rol = model.Rol;
            user.Cedula = model.Cedula;
            user.Puesto = model.Puesto;
            user.Telefono = model.Telefono;
            user.Departamento = model.Departamento;
            user.Direccion = model.Direccion;
            user.FechaNacimiento = model.FechaNacimiento;
            user.IsActive = model.Activo;
            user.UltimaActualizacion = DateTime.Now;

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                if (model.NewPassword != model.ConfirmNewPassword)
                {
                    return ServiceResult.ErrorResult("Las nuevas contraseñas no coinciden.");
                }
                user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
            }

            // Handle profile picture upload
            if (model.NuevaFotoPerfil != null)
            {
                // In a real application, you would save the file to a storage (e.g., wwwroot/images/profiles)
                // and store the path in user.FotoPerfilUrl.
                // For simplicity, let's just simulate storing the path.
                var fileName = $"{user.Id}_{Guid.NewGuid().ToString()}_{model.NuevaFotoPerfil.FileName}";
                var filePath = Path.Combine("wwwroot", "images", "profiles", fileName);
                var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.NuevaFotoPerfil.CopyToAsync(stream);
                }
                user.FotoPerfilUrl = $"/images/profiles/{fileName}";
            }
            else if (model.FotoPerfilActual == null && user.FotoPerfilUrl != null)
            {
                // If FotoPerfilActual is null and user had a photo, it means it was removed
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.FotoPerfilUrl.TrimStart('/'));
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
                user.FotoPerfilUrl = null;
            }


            _context.Usuarios.Update(user);
            await _context.SaveChangesAsync();

            await _auditService.LogAction(user.Id, "Actualización de Usuario", $"Perfil de usuario {user.NombreUsuario} actualizado.");
            return ServiceResult.SuccessResult("Usuario actualizado exitosamente.");
        }

        public async Task<ServiceResult> DeleteUser(int id)
        {
            var user = await _context.Usuarios.FindAsync(id);
            if (user == null)
            {
                return ServiceResult.ErrorResult("Usuario no encontrado.");
            }

            _context.Usuarios.Remove(user);
            await _context.SaveChangesAsync();

            await _auditService.LogAction(user.Id, "Eliminación de Usuario", $"Usuario {user.NombreUsuario} eliminado.");
            return ServiceResult.SuccessResult("Usuario eliminado exitosamente.");
        }

        public async Task<ServiceResult> ChangeUserRole(ChangeRoleViewModel model)
        {
            var user = await _context.Usuarios.FindAsync(model.UserId);
            if (user == null)
            {
                return ServiceResult.ErrorResult("Usuario no encontrado.");
            }

            if (!await _context.UserPermissions.AnyAsync(p => p.PermissionName == model.NewRole))
            {
                return ServiceResult.ErrorResult("El rol especificado no existe.");
            }

            string oldRole = user.Rol;
            user.Rol = model.NewRole;
            user.UltimaActualizacion = DateTime.Now;

            _context.Usuarios.Update(user);
            await _context.SaveChangesAsync();

            await _auditService.LogAction(user.Id, "Cambio de Rol", $"Rol de usuario {user.NombreUsuario} cambiado de {oldRole} a {model.NewRole}.");
            return ServiceResult.SuccessResult($"Rol de usuario cambiado a {model.NewRole} exitosamente.");
        }

        public async Task<ServiceResult> ChangePassword(int userId, ChangePasswordViewModel model)
        {
            var user = await _context.Usuarios.FindAsync(userId);
            if (user == null)
            {
                return ServiceResult.ErrorResult("Usuario no encontrado.");
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.CurrentPassword);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return ServiceResult.ErrorResult("La contraseña actual es incorrecta.");
            }

            if (model.NewPassword != model.ConfirmNewPassword)
            {
                return ServiceResult.ErrorResult("La nueva contraseña y la confirmación de contraseña no coinciden.");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
            user.UltimaActualizacion = DateTime.Now;

            _context.Usuarios.Update(user);
            await _context.SaveChangesAsync();

            await _auditService.LogAction(user.Id, "Cambio de Contraseña", $"Contraseña de usuario {user.NombreUsuario} cambiada.");
            return ServiceResult.SuccessResult("Contraseña cambiada exitosamente.");
        }

        public async Task<ServiceResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !user.IsActive)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return ServiceResult.SuccessResult("Si su cuenta existe y está activa, se le ha enviado un correo electrónico con instrucciones para restablecer su contraseña.");
            }

            var token = await GeneratePasswordResetTokenAsync(user);
            var resetLink = $"https://localhost:5001/Account/ResetPassword?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}"; // Replace with your actual domain

            var emailSent = await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);

            if (emailSent)
            {
                await _auditService.LogAction(user.Id, "Solicitud de Restablecimiento de Contraseña", $"Solicitud de restablecimiento de contraseña para {user.Email}.");
                return ServiceResult.SuccessResult("Si su cuenta existe y está activa, se le ha enviado un correo electrónico con instrucciones para restablecer su contraseña.");
            }
            else
            {
                return ServiceResult.ErrorResult("Hubo un problema al enviar el correo electrónico de restablecimiento de contraseña. Por favor, inténtelo de nuevo más tarde.");
            }
        }

        public async Task<ServiceResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !user.IsActive)
            {
                return ServiceResult.ErrorResult("Usuario no encontrado o inactivo.");
            }

            var passwordReset = await _context.PasswordResets
                .Where(pr => pr.UserId == user.Id && pr.Token == model.Token && pr.ExpirationDate > DateTime.UtcNow)
                .OrderByDescending(pr => pr.ExpirationDate)
                .FirstOrDefaultAsync();

            if (passwordReset == null)
            {
                return ServiceResult.ErrorResult("Token de restablecimiento de contraseña inválido o expirado.");
            }

            if (model.Password != model.ConfirmPassword)
            {
                return ServiceResult.ErrorResult("Las contraseñas no coinciden.");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
            user.UltimaActualizacion = DateTime.Now;

            _context.Usuarios.Update(user);
            _context.PasswordResets.Remove(passwordReset); // Invalidate the token
            await _context.SaveChangesAsync();

            await _auditService.LogAction(user.Id, "Restablecimiento de Contraseña", $"Contraseña de usuario {user.NombreUsuario} restablecida.");
            return ServiceResult.SuccessResult("Su contraseña ha sido restablecida exitosamente.");
        }

        public async Task<ServiceResult> ManagePermissions(ManagePermissionsViewModel model)
        {
            var user = await _context.Usuarios.FindAsync(model.UserId);
            if (user == null)
            {
                return ServiceResult.ErrorResult("Usuario no encontrado.");
            }

            // For simplicity, we are treating roles as permissions.
            // In a more complex system, you would manage a many-to-many relationship
            // between users and granular permissions.
            // Here, we just update the user's role.
            if (!string.IsNullOrEmpty(model.SelectedPermissions.FirstOrDefault()))
            {
                var newRole = model.SelectedPermissions.FirstOrDefault();
                if (newRole != null && await _context.UserPermissions.AnyAsync(p => p.PermissionName == newRole))
                {
                    string oldRole = user.Rol;
                    user.Rol = newRole;
                    user.UltimaActualizacion = DateTime.Now;
                    _context.Usuarios.Update(user);
                    await _context.SaveChangesAsync();
                    await _auditService.LogAction(user.Id, "Gestión de Permisos", $"Rol de usuario {user.NombreUsuario} cambiado de {oldRole} a {newRole}.");
                    return ServiceResult.SuccessResult($"Permisos de usuario actualizados. Nuevo rol: {newRole}.");
                }
                else
                {
                    return ServiceResult.ErrorResult("El rol seleccionado no es válido.");
                }
            }
            else
            {
                return ServiceResult.ErrorResult("Debe seleccionar al menos un rol.");
            }
        }

        public async Task<List<string>> GetUserPermissions(int userId)
        {
            var user = await _context.Usuarios.FindAsync(userId);
            if (user == null)
            {
                return new List<string>();
            }
            // In this simplified model, user's role is their permission.
            return new List<string> { user.Rol };
        }

        public async Task<List<string>> GetAllPermissions()
        {
            return await _context.UserPermissions.Select(p => p.PermissionName).ToListAsync();
        }

        public async Task<bool> IsInRoleAsync(int userId, string roleName)
        {
            var user = await _context.Usuarios.FindAsync(userId);
            return user != null && user.Rol == roleName;
        }

        public async Task<bool> IsEmailConfirmedAsync(int userId)
        {
            var user = await _context.Usuarios.FindAsync(userId);
            return user?.EmailConfirmed ?? false;
        }

        public async Task ConfirmEmailAsync(int userId)
        {
            var user = await _context.Usuarios.FindAsync(userId);
            if (user != null)
            {
                user.EmailConfirmed = true;
                user.UltimaActualizacion = DateTime.Now;
                _context.Usuarios.Update(user);
                await _context.SaveChangesAsync();
                await _auditService.LogAction(user.Id, "Confirmación de Correo", $"Correo electrónico de {user.Email} confirmado.");
            }
        }

        public async Task<string> GeneratePasswordResetTokenAsync(Usuario user)
        {
            var token = Guid.NewGuid().ToString();
            var passwordReset = new PasswordReset
            {
                UserId = user.Id,
                Token = token,
                ExpirationDate = DateTime.UtcNow.AddHours(24), // Token valid for 24 hours
                FechaCreacion = DateTime.UtcNow
            };
            _context.PasswordResets.Add(passwordReset);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<bool> VerifyPasswordResetTokenAsync(Usuario user, string token)
        {
            return await _context.PasswordResets.AnyAsync(pr => pr.UserId == user.Id && pr.Token == token && pr.ExpirationDate > DateTime.UtcNow);
        }

        public async Task<bool> CheckPasswordAsync(Usuario user, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success;
        }

        public async Task UpdateLastLogin(int userId)
        {
            var user = await _context.Usuarios.FindAsync(userId);
            if (user != null)
            {
                user.UltimoAcceso = DateTime.Now;
                _context.Usuarios.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string?> GetUserRole(int userId)
        {
            var user = await _context.Usuarios.FindAsync(userId);
            return user?.Rol;
        }
    }
}
