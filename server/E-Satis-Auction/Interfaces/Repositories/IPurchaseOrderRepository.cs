using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Models.Commerce;

namespace E_Satis_Auction.Interfaces.Repositories;

public interface IPurchaseOrderRepository : IGenericRepository<PurchaseOrder>
{
    Task<PurchaseOrder?> GetByIdWithDetailsAsync(Guid id, bool enableTracking = false, CancellationToken cancellationToken = default);
    Task<PurchaseOrder?> GetByIdempotencyKeyWithDetailsAsync(string idempotencyKey, bool enableTracking = false, CancellationToken cancellationToken = default);
    Task<bool> HasLineForProductListingAsync(Guid productListingId, CancellationToken cancellationToken = default);
}
