using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Models.Commerce;

namespace E_Satis_Auction.Interfaces.Repositories;

public interface IAuctionRepository : IGenericRepository<Auction>
{
    Task<Auction?> GetByIdWithDetailsAsync(Guid id, bool enableTracking = false, CancellationToken cancellationToken = default);
    Task<Auction?> GetByPurchaseOrderIdAsync(Guid purchaseOrderId, bool enableTracking = false, CancellationToken cancellationToken = default);
    Task<Auction?> GetByPaymentAttemptIdAsync(Guid paymentAttemptId, bool enableTracking = false, CancellationToken cancellationToken = default);
    Task<bool> HasOpenAuctionForProductListingAsync(Guid productListingId, Guid? excludedId = null, CancellationToken cancellationToken = default);
    Task<PaginatedList<Auction>> GetPublicAuctionsPaginatedAsync(AuctionStatus? status, Guid? productId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<PaginatedList<Auction>> GetAdminAuctionsPaginatedAsync(AuctionStatus? status, Guid? productListingId, Guid? productId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<List<Auction>> GetScheduledToActivateAsync(DateTimeOffset now, int take, CancellationToken cancellationToken = default);
    Task<List<Auction>> GetActiveToFinalizeAsync(DateTimeOffset now, int take, CancellationToken cancellationToken = default);
}
