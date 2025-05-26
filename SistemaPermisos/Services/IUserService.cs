using SistemaPermisos.Models;
using SistemaPermisos.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaPermisos.Services
{
    public interface IUserService
    {
        Task<IEnumerable<Usuario>> GetAllUsersAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario?> GetUserByIdAsync(int id);
        Task<Usuario?> GetByEmailAsync(string email);
        Task<Usuario?> GetUserByEmailAsync(string email);
        Task<IEnumerable<Usuario>> FindAsync(Func<Usuario, bool> predicate);

        Task<Usuario> CreateAsync(Usuario usuario);
        Task<bool> CreateUserAsync(UserCreateViewModel model);
        Task<Usuario> UpdateAsync(Usuario usuario);
        Task<bool> UpdateUserAsync(UserEditViewModel model);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteUserAsync(int id);

        Task<bool> ChangeRoleAsync(int userId, string newRole);
        Task<bool> ChangeUserRoleAsync(int userId, string newRole);
        Task<bool> ChangeUserRoleAsync(ChangeRoleViewModel model);
        Task<bool> ValidatePasswordAsync(int userId, string password);
        Task<bool> ValidateUserAsync(string email, string password);

        Task<bool> AuthenticateAsync(string email, string password);
        void Logout();

        Task<bool> ResetPasswordAsync(int userId, string newPassword);
        Task<bool> CreatePasswordResetTokenAsync(string email);
        Task<bool> InitiatePasswordResetAsync(string email);
        Task<(bool isValid, int userId)> ValidatePasswordResetTokenAsync(string token);
        Task<bool> CompletePasswordResetAsync(string token, string newPassword);
        Task<bool> MarkTokenAsUsedAsync(string token);

        Task<bool> HasPermissionAsync(int userId, string permission);
        Task<List<string>> GetUserPermissionsAsync(int userId);
        Task<bool> UpdateUserPermissionsAsync(int userId, List<string> permissions);
        Task<bool> UserHasPermissionAsync(int userId, string permission);
        Task<bool> AddPermissionAsync(int userId, string permission);
        Task<bool> RemovePermissionAsync(int userId, string permission);
        Task<bool> ToggleUserStatusAsync(int userId);

        Task<PaginatedList<Usuario>> GetPaginatedUsersAsync(int pageIndex, int pageSize, string? searchString = null, string? roleFilter = null);
    }
}
