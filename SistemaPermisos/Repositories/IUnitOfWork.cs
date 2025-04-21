using System;
using System.Threading.Tasks;
using SistemaPermisos.Models;

namespace SistemaPermisos.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Usuario> Usuarios { get; }
        IRepository<Permiso> Permisos { get; }
        IRepository<OmisionMarca> OmisionesMarca { get; }
        IRepository<ReporteDano> ReportesDanos { get; }
        IRepository<AuditLog> AuditLogs { get; }
        IRepository<UserPermission> UserPermissions { get; }
        IRepository<PasswordReset> PasswordResets { get; }
        IRepository<TwoFactorAuth> TwoFactorAuth { get; }

        Task<int> CompleteAsync();
    }
}

