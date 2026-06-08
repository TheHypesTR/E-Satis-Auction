using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Commerce;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Data.Repositories;

public sealed class AuctionBidRepository : GenericRepository<AuctionBid>, IAuctionBidRepository
{
    public AuctionBidRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<AuctionBid?> GetByIdempotencyAsync(Guid auctionId, string bidderUserId, string idempotencyKey, bool enableTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<AuctionBid> query = _dbSet.Where(bid =>
            bid.AuctionId == auctionId &&
            bid.BidderUserId == bidderUserId &&
            bid.IdempotencyKey == idempotencyKey);

        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PaginatedList<AuctionBid>> GetAcceptedBidsPaginatedAsync(Guid auctionId, DateTime auctionStartsAt, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(bid => bid.AuctionId == auctionId && bid.Status == AuctionBidStatus.Accepted && bid.CreatedAt >= auctionStartsAt)
            .OrderByDescending(bid => bid.Amount)
            .ThenBy(bid => bid.CreatedAt)
            .ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
    }
}
