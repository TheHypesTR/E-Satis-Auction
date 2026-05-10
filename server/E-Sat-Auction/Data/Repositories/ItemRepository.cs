using e_Sat_Auction.Enums;
using e_Sat_Auction.Interfaces.Repositories;
using e_Sat_Auction.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace e_Sat_Auction.Data.Repositories;

public class ItemRepository : GenericRepository<Item>, IItemRepository
{
    public ItemRepository(AppDbContext context) : base(context)
    {
    }
    
    public async Task<List<Item>> GetItemsByFacilityAndIdsAsync(Guid facilityId, IEnumerable<Guid> itemIds, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.FacilityId == facilityId && itemIds.Contains(i.Id))
            .ToListAsync(cancellationToken); 
    }
    
    public async Task<List<Item>> GetItemsByIdsAsync(IEnumerable<Guid> itemIds, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(i => itemIds.Contains(i.Id))
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Dictionary<Guid, int>> GetAvailableStockSummaryAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(i => i.ProductId == productId && i.Status == ItemStatus.Available)
            .GroupBy(i => i.FacilityId)
            .Select(g => new
                { 
                    FacilityId = g.Key,
                    TotalQuantity = g.Sum(i => i.Quantity)
                })
            .ToDictionaryAsync(x => x.FacilityId, x => x.TotalQuantity, cancellationToken);
    }
}