using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Models.Commerce;

namespace E_Satis_Auction.Interfaces.Repositories;

public interface IReturnRequestRepository : IGenericRepository<ReturnRequest>
{
    Task<ReturnRequest?> GetByIdWithLinesAsync(Guid id, bool enableTracking = false, CancellationToken cancellationToken = default);
}
