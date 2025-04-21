using System.Threading.Tasks;
using SistemaPermisos.Data;
using SistemaPermisos.Models;

namespace SistemaPermisos.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private bool _disposed = false;

        public IRepository<Usuario> Usuarios { get; private set; }
        public IRepository<Permiso> Permisos { get; private set; }
        public IRepository<OmisionMarca> OmisionesMarca { get; private set; }
        public IRepository<ReporteDano> ReportesDanos { get; private set; }
        public IRepository<AuditLog> AuditLogs { get; private set; }
        public IRepository<UserPermission> UserPermissions { get; private set; }
        public IRepository<PasswordReset> PasswordResets { get; private set; }
        public IRepository<TwoFactorAuth> TwoFactorAuth { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Usuarios = new Repository<Usuario>(_context);
            Permisos = new Repository<Permiso>(_context);
            OmisionesMarca = new Repository<OmisionMarca>(_context);
            ReportesDanos = new Repository<ReporteDano>(_context);
            AuditLogs = new Repository<AuditLog>(_context);
            UserPermissions = new Repository<UserPermission>(_context);
            PasswordResets = new Repository<PasswordReset>(_context);
            TwoFactorAuth = new Repository<TwoFactorAuth>(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }
    }
}

