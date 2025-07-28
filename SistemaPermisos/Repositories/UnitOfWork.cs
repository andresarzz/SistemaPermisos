using SistemaPermisos.Data;
using SistemaPermisos.Models;
using System;
using System.Threading.Tasks;

namespace SistemaPermisos.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IRepository<Usuario>? _usuarios;
        private IRepository<Permiso>? _permisos;
        private IRepository<OmisionMarca>? _omisionesMarca;
        private IRepository<ReporteDano>? _reportesDano;
        private IRepository<AuditLog>? _auditLogs;
        private IRepository<UserPermission>? _userPermissions;
        private IRepository<PasswordReset>? _passwordResets;
        private IRepository<TwoFactorAuth>? _twoFactorAuths;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IRepository<Usuario> Usuarios => _usuarios ??= new Repository<Usuario>(_context);
        public IRepository<Permiso> Permisos => _permisos ??= new Repository<Permiso>(_context);
        public IRepository<OmisionMarca> OmisionesMarca => _omisionesMarca ??= new Repository<OmisionMarca>(_context);
        public IRepository<ReporteDano> ReportesDano => _reportesDano ??= new Repository<ReporteDano>(_context);
        public IRepository<AuditLog> AuditLogs => _auditLogs ??= new Repository<AuditLog>(_context);
        public IRepository<UserPermission> UserPermissions => _userPermissions ??= new Repository<UserPermission>(_context);
        public IRepository<PasswordReset> PasswordResets => _passwordResets ??= new Repository<PasswordReset>(_context);
        public IRepository<TwoFactorAuth> TwoFactorAuths => _twoFactorAuths ??= new Repository<TwoFactorAuth>(_context);

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
