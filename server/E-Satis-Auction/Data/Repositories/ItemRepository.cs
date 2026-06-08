using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Items;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Data.Repositories;

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
        return await GetItemsByIdsAsync(itemIds, enableTracking: false, cancellationToken);
    }

    public async Task<List<Item>> GetItemsByIdsAsync(IEnumerable<Guid> itemIds, bool enableTracking, CancellationToken cancellationToken = default)
    {
        IQueryable<Item> query = _dbSet.Where(i => itemIds.Contains(i.Id));
        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<Item>> GetAvailableItemsForProductAsync(
        Guid productId,
        Guid facilityId,
        bool enableTracking = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Item> query = _dbSet
            .Where(i =>
                i.ProductId == productId &&
                i.FacilityId == facilityId &&
                i.Status == ItemStatus.Available &&
                i.Quantity > 0);

        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query
            .OrderBy(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetAvailableQuantityForProductAsync(
        Guid productId,
        Guid facilityId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(i =>
                i.ProductId == productId &&
                i.FacilityId == facilityId &&
                i.Status == ItemStatus.Available &&
                i.Quantity > 0)
            .SumAsync(i => i.Quantity, cancellationToken);
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
