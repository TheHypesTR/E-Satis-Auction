using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.ProductListing.GetProductListings;

using ProductListingEntity = Models.Commerce.ProductListing;

public sealed class GetProductListingsQueryHandler : IQueryHandler<GetProductListingsQuery, PaginatedList<ProductListingSummaryDto>>
{
    private readonly IProductListingRepository _productListingRepository;
    private readonly IProductRepository _productRepository;
    private readonly IFacilityRepository _facilityRepository;

    public GetProductListingsQueryHandler(
        IProductListingRepository productListingRepository,
        IProductRepository productRepository,
        IFacilityRepository facilityRepository)
    {
        _productListingRepository = productListingRepository;
        _productRepository = productRepository;
        _facilityRepository = facilityRepository;
    }

    public async Task<PaginatedList<ProductListingSummaryDto>> Handle(GetProductListingsQuery query, CancellationToken cancellationToken)
    {
        ProductFilterResult productFilter = await ResolveProductFilterAsync(query, cancellationToken);
        if (productFilter.HasProductFilter && productFilter.ProductIds.Count is 0)
        {
            return new PaginatedList<ProductListingSummaryDto>([], 0, query.PageNumber, query.PageSize);
        }

        PaginatedList<ProductListingEntity> listings = await _productListingRepository.GetPublicListingsPaginatedAsync(
            productFilter.HasProductFilter ? productFilter.ProductIds : null,
            query.SourceFacilityId,
            query.MinPrice,
            query.MaxPrice,
            DateTimeOffset.UtcNow,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        return await MapToPaginatedDtoAsync(listings, query.PageSize, cancellationToken);
    }

    private async Task<ProductFilterResult> ResolveProductFilterAsync(GetProductListingsQuery query, CancellationToken cancellationToken)
    {
        HashSet<Guid>? productIds = query.ProductId.HasValue ? [query.ProductId.Value] : null;

        if (query.CategoryId.HasValue)
        {
            List<Guid> categoryProductIds = await _productRepository.GetProductIdsByCategoryIdAsync(query.CategoryId.Value, cancellationToken);
            productIds = IntersectProductIds(productIds, categoryProductIds);
        }

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            List<Guid> searchedProductIds = await _productRepository.GetProductIdsBySearchTermAsync(query.SearchTerm.Trim().ToLowerInvariant(), cancellationToken);
            productIds = IntersectProductIds(productIds, searchedProductIds);
        }

        bool hasProductFilter = query.ProductId.HasValue || query.CategoryId.HasValue || !string.IsNullOrWhiteSpace(query.SearchTerm);
        return new ProductFilterResult(hasProductFilter, productIds?.ToList() ?? []);
    }

    private async Task<PaginatedList<ProductListingSummaryDto>> MapToPaginatedDtoAsync(
        PaginatedList<ProductListingEntity> listings,
        int pageSize,
        CancellationToken cancellationToken)
    {
        if (listings.Items.Count is 0)
        {
            return new PaginatedList<ProductListingSummaryDto>([], listings.TotalCount, listings.PageNumber, pageSize);
        }

        Dictionary<Guid, ProductListingProductEnrichmentDto> products = await _productRepository
            .GetProductListingEnrichmentsByIdsAsync(listings.Items.Select(listing => listing.ProductId), cancellationToken);
        Dictionary<Guid, string> facilities = await _facilityRepository
            .GetFacilityNamesByIdsAsync(listings.Items.Select(listing => listing.SourceFacilityId), cancellationToken);

        List<ProductListingSummaryDto> dtoList = listings.Items
            .Where(listing => products.ContainsKey(listing.ProductId) && facilities.ContainsKey(listing.SourceFacilityId))
            .Select(listing => CommerceDtoMapper.ToProductListingSummaryDto(
                listing,
                products[listing.ProductId],
                facilities[listing.SourceFacilityId]))
            .ToList();

        return new PaginatedList<ProductListingSummaryDto>(dtoList, listings.TotalCount, listings.PageNumber, pageSize);
    }

    private static HashSet<Guid> IntersectProductIds(HashSet<Guid>? currentIds, IEnumerable<Guid> nextIds)
    {
        HashSet<Guid> next = nextIds.ToHashSet();
        if (currentIds is null)
        {
            return next;
        }

        currentIds.IntersectWith(next);
        return currentIds;
    }

    private sealed record ProductFilterResult(bool HasProductFilter, IReadOnlyCollection<Guid> ProductIds);
}
