using System.Linq.Expressions;
using E_Satis_Auction.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Data.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, bool enableTracking = false,
        CancellationToken cancellationToken = default)
    {
        T? entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity is not null && !enableTracking)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }

        return entity;
    }

    public async Task<IEnumerable<T>> GetAllAsync(bool enableTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;
        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.ToListAsync(cancellationToken);
    }
    
    public IQueryable<T> GetAllAsQueryable(bool enableTracking = false)
    {
        IQueryable<T> query = _dbSet;
        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }
        return query;
    }

    public async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        bool enableTracking = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet.Where(predicate);
        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}