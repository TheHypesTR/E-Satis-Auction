using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Item;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Item.GetItemById;

using Models.Items;
using Models.Facilities;
using Models.Categories;
using Models.Products;

public sealed class GetItemByIdQueryHandler : IQueryHandler<GetItemByIdQuery, ItemDetailDto>
{
    private readonly IItemRepository _itemRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetItemByIdQueryHandler(
        IItemRepository itemRepository,
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IFacilityRepository facilityRepository,
        ICurrentUserService currentUserService)
    {
        _itemRepository = itemRepository;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _facilityRepository = facilityRepository;
        _currentUserService = currentUserService;
    }

    public async Task<ItemDetailDto> Handle(GetItemByIdQuery query, CancellationToken cancellationToken)
    {
        Item? item = await _itemRepository.GetByIdAsync(query.Id, enableTracking: false, cancellationToken);
        NotFoundException.ThrowIfNull(item, ErrorMessages.Item.EntityName, query.Id);

        Facility? facility = await _facilityRepository.GetByIdAsync(item!.FacilityId, enableTracking: false, cancellationToken);
        NotFoundException.ThrowIfNull(facility, ErrorMessages.Facility.EntityName, item.FacilityId);

        bool hasAccess = await _currentUserService.HasFacilityAccess(facility!.Id, cancellationToken);
        ForbiddenAccessException.ThrowIfFalse(
            hasAccess, 
            ErrorMessages.Facility.UnauthorizedFacilityAccess, 
            ErrorMessages.Exception.UnauthorizedAccess);

        Category? category = await _categoryRepository.GetByIdAsync(item.CategoryId, enableTracking: false, cancellationToken);
        NotFoundException.ThrowIfNull(category, ErrorMessages.Category.EntityName, item.CategoryId);

        Product? product = null;
        if (item.ProductId.HasValue)
        {
            product = await _productRepository.GetByIdAsync(item.ProductId.Value, enableTracking: false, cancellationToken);
            NotFoundException.ThrowIfNull(product, ErrorMessages.Product.EntityName, item.ProductId.Value);
        }

        string? productName = product?.Name;
        string displayName = item.Mode is Enums.ItemMode.Standardized ? productName ?? string.Empty : item.Name;

        return MapToDetailDto(item, displayName, facility!.Name, category!.Name, productName);
    }

    private static ItemDetailDto MapToDetailDto(Item item, string name, string facilityName, string categoryName, string? productName)
    {
        return new ItemDetailDto(
            item.Id,
            name,
            item.Mode,
            item.Status,
            item.Quantity,
            item.UnitOfMeasure,
            item.FacilityId,
            facilityName,
            item.CategoryId,
            categoryName,
            item.ProductId,
            productName,
            item.DynamicAttributes,
            item.CreatedAt,
            item.UpdatedAt,
            item.Version);
    }
}