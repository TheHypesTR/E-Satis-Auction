using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Models.Items;

namespace e_Sat_Auction.Interfaces.Repositories;

public interface IItemRepository : IGenericRepository<Item>
{
    Task<List<Item>> GetItemsByFacilityAndIdsAsync(Guid facilityId, IEnumerable<Guid> itemIds, CancellationToken cancellationToken = default);
    Task<List<Item>> GetItemsByIdsAsync(IEnumerable<Guid> itemIds, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, int>> GetAvailableStockSummaryAsync(Guid productId, CancellationToken cancellationToken = default);
}