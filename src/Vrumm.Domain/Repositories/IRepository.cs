using System.Linq.Expressions;
using Vrumm.Domain.Commom;

namespace Vrumm.Domain.Repositories;
public interface IRepository<T, TId> where T : Entity<TId>
{
    Task<T> GetByIdAsync(TId id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task RemoveAsync(T entity);
}