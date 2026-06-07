using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Models.Items;

namespace E_Satis_Auction.Interfaces.Repositories;

public interface IItemRepository : IGenericRepository<Item>
{
    Task<List<Item>> GetItemsByFacilityAndIdsAsync(Guid facilityId, IEnumerable<Guid> itemIds, CancellationToken cancellationToken = default);
    Task<List<Item>> GetItemsByIdsAsync(IEnumerable<Guid> itemIds, CancellationToken cancellationToken = default);
    Task<List<Item>> GetItemsByIdsAsync(IEnumerable<Guid> itemIds, bool enableTracking, CancellationToken cancellationToken = default);
    Task<List<Item>> GetAvailableItemsForProductAsync(Guid productId, Guid facilityId, bool enableTracking = false, CancellationToken cancellationToken = default);
    Task<int> GetAvailableQuantityForProductAsync(Guid productId, Guid facilityId, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, int>> GetAvailableStockSummaryAsync(Guid productId, CancellationToken cancellationToken = default);
}
