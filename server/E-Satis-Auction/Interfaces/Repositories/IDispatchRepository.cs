using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Models.Dispatches;

namespace E_Satis_Auction.Interfaces.Repositories;

public interface IDispatchRepository : IGenericRepository<Dispatch>
{
    Task<Dispatch?> GetByIdWithLineItemsAsync(Guid id, bool enableTracking = false, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, string>> GetTrackingNumbersByIdsAsync(IEnumerable<Guid> dispatchIds, CancellationToken cancellationToken = default);
}