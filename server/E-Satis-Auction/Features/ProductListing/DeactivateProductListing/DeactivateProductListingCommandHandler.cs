using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.ProductListing.DeactivateProductListing;

using ProductListingEntity = Models.Commerce.ProductListing;

public sealed class DeactivateProductListingCommandHandler : ICommandHandler<DeactivateProductListingCommand, AdminProductListingDetailDto>
{
    private readonly IProductListingRepository _productListingRepository;
    private readonly IProductRepository _productRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateProductListingCommandHandler(
        IProductListingRepository productListingRepository,
        IProductRepository productRepository,
        IFacilityRepository facilityRepository,
        IItemRepository itemRepository,
        IUnitOfWork unitOfWork)
    {
        _productListingRepository = productListingRepository;
        _productRepository = productRepository;
        _facilityRepository = facilityRepository;
        _itemRepository = itemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AdminProductListingDetailDto> Handle(DeactivateProductListingCommand command, CancellationToken cancellationToken)
    {
        ProductListingEntity? listing = await _productListingRepository.GetByIdAsync(command.Id, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(listing, ErrorMessages.ProductListing.EntityName, command.Id);

        listing!.Deactivate();
        _productListingRepository.Update(listing);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return await MapToDetailDtoAsync(listing, cancellationToken);
    }

    private async Task<AdminProductListingDetailDto> MapToDetailDtoAsync(ProductListingEntity listing, CancellationToken cancellationToken)
    {
        Dictionary<Guid, ProductListingProductEnrichmentDto> products = await _productRepository
            .GetProductListingEnrichmentsByIdsAsync([listing.ProductId], cancellationToken);
        ProductListingProductEnrichmentDto product = products[listing.ProductId];

        Dictionary<Guid, string> facilities = await _facilityRepository.GetFacilityNamesByIdsAsync([listing.SourceFacilityId], cancellationToken);
        string facilityName = facilities[listing.SourceFacilityId];

        int availableQuantity = await _itemRepository.GetAvailableQuantityForProductAsync(
            listing.ProductId,
            listing.SourceFacilityId,
            cancellationToken);

        return CommerceDtoMapper.ToAdminProductListingDetailDto(listing, product, facilityName, availableQuantity);
    }
}
