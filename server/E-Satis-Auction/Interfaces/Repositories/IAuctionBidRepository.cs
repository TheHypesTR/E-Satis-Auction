using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Models.Commerce;

namespace E_Satis_Auction.Interfaces.Repositories;

public interface IAuctionBidRepository : IGenericRepository<AuctionBid>
{
    Task<AuctionBid?> GetByIdempotencyAsync(Guid auctionId, string bidderUserId, string idempotencyKey, bool enableTracking = false, CancellationToken cancellationToken = default);
    Task<PaginatedList<AuctionBid>> GetAcceptedBidsPaginatedAsync(Guid auctionId, DateTime auctionStartsAt, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
