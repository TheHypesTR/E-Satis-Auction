using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Models.Commerce;

namespace E_Satis_Auction.Interfaces.Repositories;

public interface IProductListingRepository : IGenericRepository<ProductListing>
{
    Task<ProductListing?> GetActiveByIdAsync(Guid id, bool enableTracking = false, CancellationToken cancellationToken = default);
    Task<PaginatedList<ProductListing>> GetPublicListingsPaginatedAsync(
        IReadOnlyCollection<Guid>? productIds,
        Guid? sourceFacilityId,
        decimal? minPrice,
        decimal? maxPrice,
        DateTimeOffset now,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<PaginatedList<ProductListing>> GetAdminListingsPaginatedAsync(
        ProductListingStatus? status,
        IReadOnlyCollection<Guid>? productIds,
        Guid? sourceFacilityId,
        decimal? minPrice,
        decimal? maxPrice,
        DateTimeOffset? startDate,
        DateTimeOffset? endDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsActiveForProductAndFacilityAsync(
        Guid productId,
        Guid sourceFacilityId,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default);
}
