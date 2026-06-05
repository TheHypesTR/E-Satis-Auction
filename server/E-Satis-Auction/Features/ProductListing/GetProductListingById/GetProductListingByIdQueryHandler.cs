using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.ProductListing.GetProductListingById;

using ProductListingEntity = Models.Commerce.ProductListing;

public sealed class GetProductListingByIdQueryHandler : IQueryHandler<GetProductListingByIdQuery, ProductListingDetailDto>
{
    private readonly IProductListingRepository _productListingRepository;
    private readonly IProductRepository _productRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IItemRepository _itemRepository;

    public GetProductListingByIdQueryHandler(
        IProductListingRepository productListingRepository,
        IProductRepository productRepository,
        IFacilityRepository facilityRepository,
        IItemRepository itemRepository)
    {
        _productListingRepository = productListingRepository;
        _productRepository = productRepository;
        _facilityRepository = facilityRepository;
        _itemRepository = itemRepository;
    }

    public async Task<ProductListingDetailDto> Handle(GetProductListingByIdQuery query, CancellationToken cancellationToken)
    {
        ProductListingEntity? listing = await _productListingRepository.GetByIdAsync(query.Id, enableTracking: false, cancellationToken);
        if (listing is null || !listing.IsSellableAt(DateTimeOffset.UtcNow))
        {
            NotFoundException.ThrowIfNull(null, ErrorMessages.ProductListing.EntityName, query.Id);
        }

        Dictionary<Guid, ProductListingProductEnrichmentDto> products = await _productRepository
            .GetProductListingEnrichmentsByIdsAsync([listing!.ProductId], cancellationToken);
        ProductListingProductEnrichmentDto? product = products.GetValueOrDefault(listing.ProductId);
        NotFoundException.ThrowIfNull(product, ErrorMessages.Product.EntityName, listing.ProductId);

        Dictionary<Guid, string> facilities = await _facilityRepository.GetFacilityNamesByIdsAsync([listing.SourceFacilityId], cancellationToken);
        facilities.TryGetValue(listing.SourceFacilityId, out string? facilityName);
        NotFoundException.ThrowIfNull(facilityName, ErrorMessages.Facility.EntityName, listing.SourceFacilityId);

        int availableQuantity = await _itemRepository.GetAvailableQuantityForProductAsync(
            listing.ProductId,
            listing.SourceFacilityId,
            cancellationToken);

        return CommerceDtoMapper.ToProductListingDetailDto(listing, product!, facilityName!, availableQuantity);
    }
}
