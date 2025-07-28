using SistemaPermisos.Models;
using System.Threading.Tasks;

namespace SistemaPermisos.Services
{
    public interface IAuditService
    {
        Task LogAction(int userId, string action, string? details = null);
    }
}
