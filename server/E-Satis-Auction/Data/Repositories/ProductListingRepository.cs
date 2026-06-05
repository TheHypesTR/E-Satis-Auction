using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Commerce;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Data.Repositories;

public sealed class ProductListingRepository : GenericRepository<ProductListing>, IProductListingRepository
{
    public ProductListingRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ProductListing?> GetActiveByIdAsync(
        Guid id,
        bool enableTracking = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ProductListing> query = _dbSet.Where(listing => listing.Id == id && listing.Status == ProductListingStatus.Active);
        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PaginatedList<ProductListing>> GetPublicListingsPaginatedAsync(
        IReadOnlyCollection<Guid>? productIds,
        Guid? sourceFacilityId,
        decimal? minPrice,
        decimal? maxPrice,
        DateTimeOffset now,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ProductListing> query = _dbSet.AsNoTracking()
            .Where(listing =>
                listing.Status == ProductListingStatus.Active &&
                (!listing.ActiveFrom.HasValue || listing.ActiveFrom.Value <= now) &&
                (!listing.ActiveUntil.HasValue || listing.ActiveUntil.Value >= now));

        query = ApplySharedFilters(query, productIds, sourceFacilityId, minPrice, maxPrice);

        return await query
            .OrderByDescending(listing => listing.UpdatedAt)
            .ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
    }

    public async Task<PaginatedList<ProductListing>> GetAdminListingsPaginatedAsync(
        ProductListingStatus? status,
        IReadOnlyCollection<Guid>? productIds,
        Guid? sourceFacilityId,
        decimal? minPrice,
        decimal? maxPrice,
        DateTimeOffset? startDate,
        DateTimeOffset? endDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ProductListing> query = _dbSet.AsNoTracking();

        if (status.HasValue)
        {
            query = query.Where(listing => listing.Status == status.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(listing => listing.CreatedAt >= startDate.Value.UtcDateTime);
        }

        if (endDate.HasValue)
        {
            query = query.Where(listing => listing.CreatedAt <= endDate.Value.UtcDateTime);
        }

        query = ApplySharedFilters(query, productIds, sourceFacilityId, minPrice, maxPrice);

        return await query
            .OrderByDescending(listing => listing.CreatedAt)
            .ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
    }

    public async Task<bool> ExistsActiveForProductAndFacilityAsync(
        Guid productId,
        Guid sourceFacilityId,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(
                listing =>
                    listing.ProductId == productId &&
                    listing.SourceFacilityId == sourceFacilityId &&
                    listing.Status == ProductListingStatus.Active &&
                    (!excludedId.HasValue || listing.Id != excludedId.Value),
                cancellationToken);
    }

    private static IQueryable<ProductListing> ApplySharedFilters(
        IQueryable<ProductListing> query,
        IReadOnlyCollection<Guid>? productIds,
        Guid? sourceFacilityId,
        decimal? minPrice,
        decimal? maxPrice)
    {
        if (productIds is { Count: > 0 })
        {
            query = query.Where(listing => productIds.Contains(listing.ProductId));
        }

        if (sourceFacilityId.HasValue)
        {
            query = query.Where(listing => listing.SourceFacilityId == sourceFacilityId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(listing => listing.SalePrice >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(listing => listing.SalePrice <= maxPrice.Value);
        }

        return query;
    }
}
