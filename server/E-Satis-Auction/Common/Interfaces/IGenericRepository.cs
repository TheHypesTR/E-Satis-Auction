using System.Linq.Expressions;

namespace E_Satis_Auction.Common.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, bool enableTracking = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(bool enableTracking = false, CancellationToken cancellationToken = default);
    IQueryable<T> GetAllAsQueryable(bool enableTracking = false);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool enableTracking = false, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
}