using SistemaPermisos.Models;
using SistemaPermisos.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaPermisos.Services
{
    public interface IUserService
    {
        Task<Usuario?> GetUserById(int id);
        Task<Usuario?> GetUserByEmail(string email);
        Task<Usuario?> GetUserByUsername(string username);
        Task<PaginatedList<Usuario>> GetPaginatedUsers(int pageNumber, int pageSize, string? searchString, string? currentFilter);
        Task<ServiceResult> CreateUser(UserCreateViewModel model);
        Task<ServiceResult> UpdateUser(UserEditViewModel model);
        Task<ServiceResult> DeleteUser(int id);
        Task<ServiceResult> ChangeUserRole(ChangeRoleViewModel model);
        Task<ServiceResult> ChangePassword(int userId, ChangePasswordViewModel model);
        Task<ServiceResult> ResetPassword(ResetPasswordViewModel model);
        Task<ServiceResult> ForgotPassword(ForgotPasswordViewModel model);
        Task<ServiceResult> ManagePermissions(ManagePermissionsViewModel model);
        Task<List<string>> GetUserPermissions(int userId);
        Task<List<string>> GetAllPermissions();
        Task<bool> IsInRoleAsync(int userId, string roleName);
        Task<bool> IsEmailConfirmedAsync(int userId);
        Task ConfirmEmailAsync(int userId);
        Task<string> GeneratePasswordResetTokenAsync(Usuario user);
        Task<bool> VerifyPasswordResetTokenAsync(Usuario user, string token);
        Task<bool> CheckPasswordAsync(Usuario user, string password);
        Task UpdateLastLogin(int userId);
        Task<string?> GetUserRole(int userId);
    }
}
