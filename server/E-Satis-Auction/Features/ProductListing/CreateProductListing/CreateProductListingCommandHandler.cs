using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.ProductListing.CreateProductListing;

using FacilityEntity = Models.Facilities.Facility;
using ProductEntity = Models.Products.Product;
using ProductListingEntity = Models.Commerce.ProductListing;

public sealed class CreateProductListingCommandHandler : ICommandHandler<CreateProductListingCommand, AdminProductListingDetailDto>
{
    private readonly IProductListingRepository _productListingRepository;
    private readonly IProductRepository _productRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductListingCommandHandler(
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

    public async Task<AdminProductListingDetailDto> Handle(CreateProductListingCommand command, CancellationToken cancellationToken)
    {
        ProductEntity? product = await _productRepository.GetByIdAsync(command.ProductId, enableTracking: false, cancellationToken);
        NotFoundException.ThrowIfNull(product, ErrorMessages.Product.EntityName, command.ProductId);

        BusinessException.ThrowIfFalse(
            product!.IsActive,
            ErrorMessages.Product.ProductNotAvailable,
            ErrorMessages.Exception.ProductTitle);

        FacilityEntity? facility = await _facilityRepository.GetByIdAsync(command.SourceFacilityId, enableTracking: false, cancellationToken);
        NotFoundException.ThrowIfNull(facility, ErrorMessages.Facility.EntityName, command.SourceFacilityId);

        ProductListingEntity listing = ProductListingEntity.Create(
            command.ProductId,
            command.SourceFacilityId,
            command.Price,
            command.Currency,
            command.ActiveFrom,
            command.ActiveUntil);

        await _productListingRepository.AddAsync(listing, cancellationToken);
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
