using SistemaPermisos.Models;
using SistemaPermisos.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaPermisos.Services
{
    public interface IUserService
    {
        Task<IEnumerable<Usuario>> GetAllUsersAsync();
        Task<Usuario> GetByIdAsync(int id);
        Task<Usuario> GetByEmailAsync(string email);
        Task<bool> CreateAsync(Usuario usuario);
        Task<bool> UpdateAsync(Usuario usuario);
        Task<bool> DeleteAsync(int id);
        Task<bool> ChangeRoleAsync(int userId, string newRole);
        Task<bool> ToggleActiveStatusAsync(int userId);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> AuthenticateAsync(string email, string password);
        void Logout();
        Task<bool> ResetPasswordAsync(int userId, string newPassword);
        Task<bool> CreatePasswordResetTokenAsync(string email);
        Task<(bool isValid, int userId)> ValidatePasswordResetTokenAsync(string token);
        Task<bool> MarkTokenAsUsedAsync(string token);
        Task<IEnumerable<Usuario>> FindAsync(Func<Usuario, bool> predicate);
        Task<bool> GetUserByIdAsync(int id, out Usuario usuario);
        Task<bool> GetUserByEmailAsync(string email, out Usuario usuario);
        Task<bool> CreateUserAsync(UserCreateViewModel model);
        Task<bool> UpdateUserAsync(UserEditViewModel model);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ChangeUserRoleAsync(ChangeRoleViewModel model);
        Task<bool> ToggleUserStatusAsync(int id);
        Task<bool> ValidatePasswordAsync(int userId, string password);
        Task<bool> HasPermissionAsync(int userId, string permission);
        Task<IEnumerable<string>> GetUserPermissionsAsync(int userId);
        Task<bool> AddPermissionAsync(int userId, string permission);
        Task<bool> RemovePermissionAsync(int userId, string permission);
        Task<bool> InitiatePasswordResetAsync(string email);
        Task<bool> CompletePasswordResetAsync(string token, string newPassword);
        Task<bool> Enable2FAAsync(int userId);
        Task<bool> Disable2FAAsync(int userId);
        Task<string> Generate2FACodeAsync(int userId);
        Task<bool> Verify2FACodeAsync(int userId, string code);
    }
}
