using SistemaPermisos.Models;
using SistemaPermisos.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaPermisos.Services
{
    public interface IUserService
    {
        Task<IEnumerable<Usuario>> GetAllUsersAsync();
        Task<Usuario> GetUserByIdAsync(int id);
        Task<Usuario> GetUserByEmailAsync(string email);
        Task<bool> CreateUserAsync(UserCreateViewModel model);
        Task<bool> UpdateUserAsync(UserEditViewModel model);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ChangeUserRoleAsync(ChangeRoleViewModel model);
        Task<bool> ToggleUserStatusAsync(int id);
        Task<bool> ChangePasswordAsync(int userId, string newPassword);
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

