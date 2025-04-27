using System.Linq.Expressions;
using Vrumm.Domain.Common;

namespace Vrumm.Domain.Repositories;
public interface IRepository<T, TId> where T : Entity<TId>
{
    Task<T> GetByIdAsync(TId id, CancellationToken cancellationToken);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
    Task AddAsync(T entity, CancellationToken cancellationToken);
    Task UpdateAsync(T entity);
    Task RemoveAsync(T entity);
}