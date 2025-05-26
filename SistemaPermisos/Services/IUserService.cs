using SistemaPermisos.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaPermisos.Services
{
    public interface IUserService
    {
        Task<IEnumerable<Usuario>> GetAllUsersAsync();
        Task<Usuario> GetUserByIdAsync(int id);
        Task<Usuario> GetUserByEmailAsync(string email);
        Task<bool> CreateUserAsync(Usuario user, string password);
        Task<bool> UpdateUserAsync(Usuario user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> ValidatePasswordAsync(string email, string password);
        Task<bool> IsEmailUniqueAsync(string email, int? userId = null);
        Task<bool> ChangeUserRoleAsync(int userId, string newRole);
        Task<bool> UpdateUserPermissionsAsync(int userId, List<int> permissionIds);
        Task<IEnumerable<UserPermission>> GetUserPermissionsAsync(int userId);
        Task<string> GeneratePasswordResetTokenAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
        Task<bool> DeactivateUserAsync(int id);
        Task<bool> ActivateUserAsync(int id);
        Task<string> GetUserFotoPerfilAsync(int id);
        Task<bool> UpdateUserFotoPerfilAsync(int id, string fotoPerfil);
    }
}
