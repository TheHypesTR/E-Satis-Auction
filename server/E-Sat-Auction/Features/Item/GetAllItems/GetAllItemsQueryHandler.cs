using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Common.Models;
using e_Sat_Auction.Dtos.Item;
using e_Sat_Auction.Interfaces;
using e_Sat_Auction.Interfaces.Repositories;
using e_Sat_Auction.Common.Extensions;

namespace e_Sat_Auction.Features.Item.GetAllItems;

using Models.Items;

public sealed class GetAllItemsQueryHandler : IQueryHandler<GetAllItemsQuery, PaginatedList<ItemSummaryDto>>
{
    private readonly IItemRepository _itemRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IFacilityManagerRepository _facilityManagerRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetAllItemsQueryHandler(
        IItemRepository itemRepository,
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IFacilityRepository facilityRepository,
        IFacilityManagerRepository facilityManagerRepository,
        ICurrentUserService currentUserService)
    {
        _itemRepository = itemRepository;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _facilityRepository = facilityRepository;
        _facilityManagerRepository = facilityManagerRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedList<ItemSummaryDto>> Handle(GetAllItemsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Item> itemsQuery = _itemRepository.GetAllAsQueryable();
        itemsQuery = await ApplyAuthorizationAsync(itemsQuery, cancellationToken);
        itemsQuery = await ApplyFiltersAsync(itemsQuery, query, cancellationToken);

        PaginatedList<Item> pagedItems = await itemsQuery
            .OrderByDescending(i => i.UpdatedAt)
            .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

        if (pagedItems.Items.Count is 0)
        {
            return new PaginatedList<ItemSummaryDto>([], pagedItems.TotalCount, query.PageNumber, query.PageSize);
        }

        IReadOnlyCollection<Item> items = pagedItems.Items;
        Dictionary<Guid, string> categoryNames = await _categoryRepository
            .GetCategoryNamesByIdsAsync(items.Select(i => i.CategoryId).Distinct(), cancellationToken);

        Dictionary<Guid, string> facilityNames = await _facilityRepository
            .GetFacilityNamesByIdsAsync(items.Select(i => i.FacilityId).Distinct(), cancellationToken);

        List<Guid> productIds = items
            .Where(i => i.ProductId.HasValue)
            .Select(i => i.ProductId!.Value)
            .Distinct()
            .ToList();

        Dictionary<Guid, string> productNames = productIds.Count is 0
            ? new Dictionary<Guid, string>()
            : await _productRepository.GetProductNamesByIdsAsync(productIds, cancellationToken);

        List<ItemSummaryDto> dtoList = items.Select(item => MapToSummaryDto(item, productNames, categoryNames, facilityNames)).ToList();

        return new PaginatedList<ItemSummaryDto>(dtoList, pagedItems.TotalCount, pagedItems.PageNumber, query.PageSize);
    }

    private async Task<IQueryable<Item>> ApplyAuthorizationAsync(IQueryable<Item> query, CancellationToken cancellationToken)
    {
        if (_currentUserService.IsGeneralAdmin)
        {
            return query;
        }

        List<Guid> facilityIds = await _facilityManagerRepository
            .GetFacilityIdsByUserIdAsync(_currentUserService.UserId, cancellationToken);

        if (facilityIds.Count is 0)
        {
            return query.Where(_ => false);
        }

        return query.Where(i => facilityIds.Contains(i.FacilityId));
    }

    private async Task<IQueryable<Item>> ApplyFiltersAsync(IQueryable<Item> query, GetAllItemsQuery filters, CancellationToken cancellationToken)
    {
        if (filters.Status.HasValue)
        {
            query = query.Where(i => i.Status == filters.Status.Value);
        }

        if (filters.Mode.HasValue)
        {
            query = query.Where(i => i.Mode == filters.Mode.Value);
        }

        if (filters.FacilityId.HasValue)
        {
            query = query.Where(i => i.FacilityId == filters.FacilityId.Value);
        }

        if (filters.CategoryId.HasValue)
        {
            query = query.Where(i => i.CategoryId == filters.CategoryId.Value);
        }

        if (filters.ProductId.HasValue)
        {
            query = query.Where(i => i.ProductId == filters.ProductId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
        {
            string search = filters.SearchTerm.Trim().ToLower();
            List<Guid> matchingProductIds = await _productRepository.GetProductIdsBySearchTermAsync(search, cancellationToken);

            query = query.Where(i =>
                i.Name.ToLower().Contains(search) ||
                (i.ProductId.HasValue && matchingProductIds.Contains(i.ProductId.Value)));
        }

        return query;
    }

    private static ItemSummaryDto MapToSummaryDto(
        Item item,
        Dictionary<Guid, string> productNames,
        Dictionary<Guid, string> categoryNames,
        Dictionary<Guid, string> facilityNames)
    {
        string? productName = item.ProductId.HasValue ? productNames.GetValueOrDefault(item.ProductId.Value) : null;
        string displayName = item.Mode is Enums.ItemMode.Standardized ? productName ?? string.Empty : item.Name;

        return new ItemSummaryDto(
            item.Id,
            displayName,
            item.Mode,
            item.Status,
            item.Quantity,
            item.UnitOfMeasure,
            item.FacilityId,
            facilityNames.GetValueOrDefault(item.FacilityId, string.Empty),
            item.CategoryId,
            categoryNames.GetValueOrDefault(item.CategoryId, string.Empty),
            item.ProductId,
            productName,
            item.CreatedAt,
            item.UpdatedAt);
    }
}