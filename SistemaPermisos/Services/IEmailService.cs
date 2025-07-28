using System.Threading.Tasks;

namespace SistemaPermisos.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string message);
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink);
    }
}
