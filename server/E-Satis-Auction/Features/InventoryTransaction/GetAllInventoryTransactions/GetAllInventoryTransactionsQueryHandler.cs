using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.InventoryTransaction;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Features.InventoryTransaction.GetAllInventoryTransactions;

using Models.InventoryTransactions;
using Models.Items;

public class GetAllInventoryTransactionsQueryHandler : IQueryHandler<GetAllInventoryTransactionsQuery, PaginatedList<InventoryTransactionDto>>
{
    private readonly IInventoryTransactionRepository _inventoryTransactionRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IDispatchRepository _dispatchRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly ICurrentUserService _currentUserService;
    
    public GetAllInventoryTransactionsQueryHandler(
        IInventoryTransactionRepository inventoryTransactionRepository,
        IFacilityRepository facilityRepository,
        IItemRepository itemRepository,
        IProductRepository productRepository,
        IDispatchRepository _dispatchRepository,
        UserManager<AppUser> userManager,
        ICurrentUserService currentUserService)
    {
        _inventoryTransactionRepository = inventoryTransactionRepository;
        _facilityRepository = facilityRepository;
        _itemRepository = itemRepository;
        _productRepository = productRepository;
        this._dispatchRepository = _dispatchRepository;
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedList<InventoryTransactionDto>> Handle(GetAllInventoryTransactionsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<InventoryTransaction> transactionQuery = _inventoryTransactionRepository.GetAllAsQueryable();
        transactionQuery = await ApplyAuthorizationAsync(transactionQuery, query.FacilityId, cancellationToken);
        transactionQuery = ApplyFilters(transactionQuery, query);
        
        PaginatedList<InventoryTransaction> pagedTransactions = await transactionQuery
            .OrderByDescending(t => t.CreatedAt)
            .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);
        
        if (pagedTransactions.Items.Count is 0)
        {
            return new PaginatedList<InventoryTransactionDto>([], pagedTransactions.TotalCount, query.PageNumber, query.PageSize);
        }
        
        List<InventoryTransaction> transactions = pagedTransactions.Items.ToList();
        
        EnrichmentData enrichmentData = await LoadEnrichmentDataAsync(transactions, cancellationToken);
        
        List<InventoryTransactionDto> dtoList = transactions
            .Select(t => MapToDto(t, enrichmentData))
            .ToList();

        return new PaginatedList<InventoryTransactionDto>(dtoList, pagedTransactions.TotalCount, pagedTransactions.PageNumber, query.PageSize);
    }
    
    private async Task<IQueryable<InventoryTransaction>> ApplyAuthorizationAsync(
        IQueryable<InventoryTransaction> transactionQuery,
        Guid? requestedFacilityId,
        CancellationToken cancellationToken)
    {
        if (_currentUserService.IsGeneralAdmin)
        {
            return requestedFacilityId.HasValue
                ? transactionQuery.Where(t => t.FacilityId == requestedFacilityId.Value)
                : transactionQuery;
        }

        if (requestedFacilityId.HasValue)
        {
            bool hasAccess = await _currentUserService.HasFacilityAccess(requestedFacilityId.Value, cancellationToken);
            ForbiddenAccessException.ThrowIfFalse(
                hasAccess,
                ErrorMessages.Facility.UnauthorizedFacilityAccess,
                ErrorMessages.Exception.UnauthorizedAccess);

            return transactionQuery.Where(t => t.FacilityId == requestedFacilityId.Value);
        }

        IReadOnlyCollection<Guid> accessibleFacilityIds = await _currentUserService.GetAccessibleFacilityIdsAsync(cancellationToken);
        return accessibleFacilityIds.Count is 0
            ? transactionQuery.Where(_ => false)
            : transactionQuery.Where(t => accessibleFacilityIds.Contains(t.FacilityId));
    }

    private static IQueryable<InventoryTransaction> ApplyFilters(IQueryable<InventoryTransaction> transactionQuery, GetAllInventoryTransactionsQuery query)
    {
        if (query.ItemId.HasValue)
        {
            transactionQuery = transactionQuery.Where(t => t.ItemId == query.ItemId.Value);
        }

        if (query.TransactionType.HasValue)
        {
            transactionQuery = transactionQuery.Where(t => t.TransactionType == query.TransactionType.Value);
        }

        if (query.ReferenceId.HasValue)
        {
            transactionQuery = transactionQuery.Where(t => t.ReferenceId == query.ReferenceId.Value);
        }

        if (query.StartDate.HasValue)
        {
            transactionQuery = transactionQuery.Where(t => t.CreatedAt >= query.StartDate.Value.UtcDateTime);
        }

        if (query.EndDate.HasValue)
        {
            transactionQuery = transactionQuery.Where(t => t.CreatedAt <= query.EndDate.Value.UtcDateTime);
        }

        return transactionQuery;
    }
    
    private async Task<EnrichmentData> LoadEnrichmentDataAsync(List<InventoryTransaction> transactions, CancellationToken cancellationToken)
    {
        List<Guid> facilityIds = transactions.Select(t => t.FacilityId).Distinct().ToList();
        Dictionary<Guid, string> facilityNames = facilityIds.Count is 0
            ? []
            : await _facilityRepository.GetFacilityNamesByIdsAsync(facilityIds, cancellationToken);

        List<Guid> itemIds = transactions.Select(t => t.ItemId).Distinct().ToList();
        List<Item> items = itemIds.Count is 0 ? [] : await _itemRepository.GetItemsByIdsAsync(itemIds, cancellationToken);
        Dictionary<Guid, Item> itemLookup = items.ToDictionary(i => i.Id, i => i);
        
        List<Guid> productIds = items
            .Where(i => i is { Mode: ItemMode.Standardized, ProductId: not null })
            .Select(i => i.ProductId!.Value)
            .Distinct()
            .ToList();
            
        Dictionary<Guid, string> productNames = productIds.Count is 0 
            ? [] 
            : await _productRepository.GetProductNamesByIdsAsync(productIds, cancellationToken);
        
        List<Guid> dispatchIds = transactions
            .Where(t => t.ReferenceId.HasValue && IsDispatchRelated(t.TransactionType))
            .Select(t => t.ReferenceId!.Value)
            .Distinct()
            .ToList();
        
        Dictionary<Guid, string> dispatchTrackingNumbers = dispatchIds.Count is 0 
            ? [] 
            : await _dispatchRepository.GetTrackingNumbersByIdsAsync(dispatchIds, cancellationToken);
        
        List<string> userIds = transactions
            .Select(t => t.CreatedBy)
            .Where(id => id is not SystemConstants.SystemUser && !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .ToList();

        Dictionary<string, string> userNames = await GetUserNamesAsync(userIds);

        return new EnrichmentData(facilityNames, itemLookup, productNames, dispatchTrackingNumbers, userNames);
    }
    
    private async Task<Dictionary<string, string>> GetUserNamesAsync(List<string> userIds)
    {
        if (userIds.Count is 0) return [];

        var users = await _userManager.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, FullName = $"{u.FirstName} {u.LastName}" })
            .ToListAsync();

        return users.ToDictionary(u => u.Id, u => u.FullName);
    }
    
    private static bool IsDispatchRelated(InventoryTransactionType type)
    {
        return type is InventoryTransactionType.Reserved 
            or InventoryTransactionType.Dispatched 
            or InventoryTransactionType.Received 
            or InventoryTransactionType.Cancelled;
    }

    private static InventoryTransactionDto MapToDto(InventoryTransaction transaction, EnrichmentData data)
    {
        string facilityName = data.FacilityNames.GetValueOrDefault(transaction.FacilityId, ErrorMessages.Facility.UnknownFacility);

        string? trackingNumber = null;
        if (transaction.ReferenceId.HasValue && IsDispatchRelated(transaction.TransactionType))
        {
            trackingNumber = data.DispatchTrackingNumbers.GetValueOrDefault(transaction.ReferenceId.Value);
        }
        
        string createdByUserName = transaction.CreatedBy is SystemConstants.SystemUser
            ? ErrorMessages.User.SystemUser
            : data.UserNames.GetValueOrDefault(transaction.CreatedBy, ErrorMessages.User.UnknownUser);
        
        data.ItemLookup.TryGetValue(transaction.ItemId, out Item? item);
        string itemName = item is null ? ErrorMessages.Item.UnknownItem : GetDisplayNameForItem(item, data.ProductNames);
        UnitOfMeasure uom = item?.UnitOfMeasure ?? UnitOfMeasure.Piece;

        return new InventoryTransactionDto(
            transaction.Id,
            transaction.ItemId,
            itemName,
            uom,
            transaction.FacilityId,
            facilityName,
            transaction.TransactionType,
            transaction.QuantityChange,
            transaction.PreviousQuantity,
            transaction.NewQuantity,
            transaction.ReferenceId,
            trackingNumber,
            transaction.CreatedBy,
            createdByUserName,
            transaction.CreatedAt);
    }
    
    private static string GetDisplayNameForItem(Item item, Dictionary<Guid, string> productNames)
    {
        if (item.Mode is ItemMode.AdHoc)
        {
            return item.Name;
        }

        return item.ProductId.HasValue ? productNames.GetValueOrDefault(item.ProductId.Value, ErrorMessages.Product.UnknownProduct) : item.Name;
    }
}