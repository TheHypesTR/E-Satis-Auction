using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Dispatches;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Data.Repositories;

public class DispatchRepository : GenericRepository<Dispatch>, IDispatchRepository
{
    public DispatchRepository(AppDbContext context) : base(context)
    {
    }
    
    public async Task<Dispatch?> GetByIdWithLineItemsAsync(Guid id, bool enableTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Dispatch> query = _dbSet.Include(d => d.LineItems);
        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }
    
    public async Task<Dictionary<Guid, string>> GetTrackingNumbersByIdsAsync(IEnumerable<Guid> dispatchIds, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(d => dispatchIds.Contains(d.Id))
            .ToDictionaryAsync(d => d.Id, d => d.TrackingNumber, cancellationToken);
    }
}