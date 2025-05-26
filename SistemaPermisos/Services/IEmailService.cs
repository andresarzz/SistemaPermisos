using System.Threading.Tasks;

namespace SistemaPermisos.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task<bool> SendPasswordResetEmailAsync(string to, string resetLink);
        Task<bool> SendWelcomeEmailAsync(string to, string userName);
        Task<bool> SendNotificationAsync(string to, string subject, string message);
        Task<bool> Send2FACodeAsync(string to, string code);
    }
}
