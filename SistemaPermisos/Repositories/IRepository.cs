using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SistemaPermisos.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        IQueryable<T> GetAll();
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> ToListAsync(IQueryable<T> query);
    }
}
