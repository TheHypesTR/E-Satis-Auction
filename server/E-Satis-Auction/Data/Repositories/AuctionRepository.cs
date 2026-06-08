using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Commerce;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Data.Repositories;

public sealed class AuctionRepository : GenericRepository<Auction>, IAuctionRepository
{
    public AuctionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Auction?> GetByIdWithDetailsAsync(Guid id, bool enableTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Auction> query = _dbSet
            .Include(auction => auction.Bids)
            .Include(auction => auction.Reservations);

        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(auction => auction.Id == id, cancellationToken);
    }

    public async Task<Auction?> GetByPurchaseOrderIdAsync(Guid purchaseOrderId, bool enableTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Auction> query = _dbSet
            .Include(auction => auction.Reservations)
            .Where(auction => auction.PurchaseOrderId == purchaseOrderId);

        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Auction?> GetByPaymentAttemptIdAsync(Guid paymentAttemptId, bool enableTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Auction> query = _dbSet
            .Include(auction => auction.Reservations)
            .Where(auction => auction.PaymentAttemptId == paymentAttemptId);

        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> HasOpenAuctionForProductListingAsync(Guid productListingId, Guid? excludedId = null, CancellationToken cancellationToken = default)
    {
        AuctionStatus[] openStatuses =
        [
            AuctionStatus.Draft,
            AuctionStatus.Scheduled,
            AuctionStatus.Active,
            AuctionStatus.Ended,
            AuctionStatus.PaymentPending
        ];

        return await _dbSet
            .AsNoTracking()
            .AnyAsync(
                auction =>
                    auction.ProductListingId == productListingId &&
                    openStatuses.Contains(auction.Status) &&
                    (!excludedId.HasValue || auction.Id != excludedId.Value),
                cancellationToken);
    }

    public async Task<PaginatedList<Auction>> GetPublicAuctionsPaginatedAsync(
        AuctionStatus? status,
        Guid? productId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        AuctionStatus[] publicStatuses =
        [
            AuctionStatus.Scheduled,
            AuctionStatus.Active,
            AuctionStatus.Ended,
            AuctionStatus.PaymentPending,
            AuctionStatus.Completed,
            AuctionStatus.Relistable
        ];

        IQueryable<Auction> query = _dbSet.AsNoTracking()
            .Where(auction => publicStatuses.Contains(auction.Status));

        if (status.HasValue)
        {
            query = query.Where(auction => auction.Status == status.Value);
        }

        if (productId.HasValue)
        {
            query = query.Where(auction => auction.ProductId == productId.Value);
        }

        return await query
            .OrderByDescending(auction => auction.Status == AuctionStatus.Active)
            .ThenBy(auction => auction.EndsAt)
            .ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
    }

    public async Task<PaginatedList<Auction>> GetAdminAuctionsPaginatedAsync(
        AuctionStatus? status,
        Guid? productListingId,
        Guid? productId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Auction> query = _dbSet.AsNoTracking();

        if (status.HasValue)
        {
            query = query.Where(auction => auction.Status == status.Value);
        }

        if (productListingId.HasValue)
        {
            query = query.Where(auction => auction.ProductListingId == productListingId.Value);
        }

        if (productId.HasValue)
        {
            query = query.Where(auction => auction.ProductId == productId.Value);
        }

        return await query
            .OrderByDescending(auction => auction.CreatedAt)
            .ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
    }

    public async Task<List<Auction>> GetScheduledToActivateAsync(DateTimeOffset now, int take, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(auction => auction.Status == AuctionStatus.Scheduled && auction.StartsAt <= now && auction.EndsAt > now)
            .OrderBy(auction => auction.StartsAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Auction>> GetActiveToFinalizeAsync(DateTimeOffset now, int take, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(auction => auction.Reservations)
            .Where(auction => auction.Status == AuctionStatus.Active && auction.EndsAt <= now)
            .OrderBy(auction => auction.EndsAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
