using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Models.Dispatches;

namespace e_Sat_Auction.Interfaces.Repositories;

public interface IDispatchRepository : IGenericRepository<Dispatch>
{
    Task<Dispatch?> GetByIdWithLineItemsAsync(Guid id, bool enableTracking = false, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, string>> GetTrackingNumbersByIdsAsync(IEnumerable<Guid> dispatchIds, CancellationToken cancellationToken = default);
}