using SistemaPermisos.Models;
using System;
using System.Threading.Tasks;

namespace SistemaPermisos.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Usuario> Usuarios { get; }
        IRepository<Permiso> Permisos { get; }
        IRepository<OmisionMarca> OmisionesMarca { get; }
        IRepository<ReporteDano> ReportesDano { get; }
        IRepository<AuditLog> AuditLogs { get; }
        IRepository<UserPermission> UserPermissions { get; }
        IRepository<PasswordReset> PasswordResets { get; }
        IRepository<TwoFactorAuth> TwoFactorAuths { get; }

        Task<int> SaveAsync();
    }
}
