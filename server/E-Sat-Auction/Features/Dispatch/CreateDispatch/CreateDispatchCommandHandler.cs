using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Exceptions;
using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.Dispatch.Requests;
using e_Sat_Auction.Enums;
using e_Sat_Auction.Interfaces;
using e_Sat_Auction.Interfaces.Repositories;

namespace e_Sat_Auction.Features.Dispatch.CreateDispatch;

using Models.Dispatches;
using Models.Items;

public class CreateDispatchCommandHandler : ICommandHandler<CreateDispatchCommand, Guid>
{
    private readonly IDispatchRepository _dispatchRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateDispatchCommandHandler(
        IDispatchRepository dispatchRepository,
        IItemRepository itemRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _dispatchRepository = dispatchRepository;
        _itemRepository = itemRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateDispatchCommand command, CancellationToken cancellationToken)
    {
        bool hasFacilityAccess = await _currentUserService.HasFacilityAccess(command.SourceFacilityId, cancellationToken);
        ForbiddenAccessException.ThrowIfFalse(
            hasFacilityAccess,
            ErrorMessages.Facility.UnauthorizedFacilityAccess,
            ErrorMessages.Exception.UnauthorizedAccess);

        List<Guid> itemIds = command.Payload.Items.Select(i => i.ItemId).Distinct().ToList();
        List<Item> items = await _itemRepository.GetItemsByFacilityAndIdsAsync(command.SourceFacilityId, itemIds, cancellationToken);

        Dictionary<Guid, Item> itemLookup = items.ToDictionary(i => i.Id, i => i);
        Dictionary<Guid, string> productNameLookup = await LoadProductNamesAsync(items, cancellationToken);

        Dispatch dispatch = Dispatch.Create(
            command.SourceFacilityId,
            command.Payload.TargetFacilityId,
            command.Payload.TargetAddressId,
            command.Payload.ReceiverName,
            command.Payload.ReceiverPhone,
            command.Payload.Notes);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            foreach (DispatchItemRequest request in command.Payload.Items)
            {
                itemLookup.TryGetValue(request.ItemId, out Item? item);
                NotFoundException.ThrowIfNull(item, ErrorMessages.Item.EntityName, request.ItemId);
                
                BusinessException.ThrowIfTrue(
                    item!.Status is not ItemStatus.Available,
                    ErrorMessages.Dispatch.ItemStatusInvalid,
                    ErrorMessages.Exception.DispatchTitle);

                item.DecreaseQuantity(request.Quantity, InventoryTransactionType.Dispatched, dispatch.Id);
                _itemRepository.Update(item);

                Dictionary<string, string> attributes = item.DynamicAttributes
                    .ToDictionary(entry => entry.Key, entry => entry.Value);

                Item reservedItem = CreateReservedItem(item, request.Quantity, attributes, dispatch.Id);
                await _itemRepository.AddAsync(reservedItem, cancellationToken);

                string itemNameSnapshot = ResolveItemNameSnapshot(item, productNameLookup);
                dispatch.AddLineItem(reservedItem.Id, item.Id, itemNameSnapshot, request.Quantity);
            }

            await _dispatchRepository.AddAsync(dispatch, cancellationToken);
            
            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return dispatch.Id;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task<Dictionary<Guid, string>> LoadProductNamesAsync(IEnumerable<Item> items, CancellationToken cancellationToken)
    {
        List<Guid> productIds = items
            .Where(i => i is { Mode: ItemMode.Standardized, ProductId: not null })
            .Select(i => i.ProductId!.Value)
            .Distinct()
            .ToList();

        return productIds.Count is 0 ? [] : await _productRepository.GetProductNamesByIdsAsync(productIds, cancellationToken);
    }

    private static Item CreateReservedItem(
        Item item,
        int quantity,
        Dictionary<string, string> attributes,
        Guid dispatchId)
    {
        return item.Mode is ItemMode.Standardized
            ? Item.CreateFromProduct(
                item.ProductId!.Value,
                item.CategoryId,
                item.FacilityId,
                quantity,
                item.UnitOfMeasure,
                ItemStatus.Reserved,
                attributes,
                InventoryTransactionType.Reserved,
                dispatchId)
            : Item.CreateAdHoc(
                item.CategoryId,
                item.FacilityId,
                item.Name,
                quantity,
                item.UnitOfMeasure,
                ItemStatus.Reserved,
                attributes,
                InventoryTransactionType.Reserved,
                dispatchId);
    }

    private static string ResolveItemNameSnapshot(Item item, Dictionary<Guid, string> productNameLookup)
    {
        if (item.Mode is ItemMode.AdHoc)
        {
            return item.Name;
        }

        Guid productId = item.ProductId!.Value;
        productNameLookup.TryGetValue(productId, out string? productName);
        NotFoundException.ThrowIfNull(productName, ErrorMessages.Product.EntityName, productId);
        
        return productName!;
    }
}