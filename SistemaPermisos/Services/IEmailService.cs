using System.Threading.Tasks;

namespace SistemaPermisos.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task SendWelcomeEmailAsync(string to, string name);
        Task SendPasswordResetEmailAsync(string to, string name, string resetLink);
        Task Send2FACodeAsync(string to, string name, string code);
        Task SendPermissionApprovedEmailAsync(string to, string name, int permisoId);
        Task SendPermissionRejectedEmailAsync(string to, string name, int permisoId, string reason);
    }
}

