using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Dispatch.Requests;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Dispatch.ReceiveDispatch;

using Models.Dispatches;
using Models.Items;

public sealed class ReceiveDispatchCommandHandler : ICommandHandler<ReceiveDispatchCommand>
{
    private readonly IDispatchRepository _dispatchRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ReceiveDispatchCommandHandler(
        IDispatchRepository dispatchRepository,
        IItemRepository itemRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _dispatchRepository = dispatchRepository;
        _itemRepository = itemRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task Handle(ReceiveDispatchCommand command, CancellationToken cancellationToken)
    {
        Dispatch? dispatch = await _dispatchRepository.GetByIdWithLineItemsAsync(command.DispatchId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(dispatch, ErrorMessages.Dispatch.EntityName, command.DispatchId);

        BusinessException.ThrowIfTrue(
            dispatch!.TargetFacilityId is null,
            ErrorMessages.Dispatch.TargetFacilityRequiredForReceipt,
            ErrorMessages.Exception.DispatchTitle);

        bool hasFacilityAccess = await _currentUserService.HasFacilityAccess(dispatch.TargetFacilityId!.Value, cancellationToken);
        ForbiddenAccessException.ThrowIfFalse(
            hasFacilityAccess,
            ErrorMessages.Facility.UnauthorizedFacilityAccess,
            ErrorMessages.Exception.UnauthorizedAccess);

        BusinessException.ThrowIfTrue(
            dispatch.Status is not DispatchStatus.InTransit,
            ErrorMessages.Dispatch.StatusNotInTransit,
            ErrorMessages.Exception.DispatchTitle);
        
        BusinessException.ThrowIfTrue(
            dispatch.LineItems.Count != command.Payload.Items.Count,
            ErrorMessages.Dispatch.ReceiptItemMissing, 
            ErrorMessages.Exception.DispatchTitle);

        Dictionary<Guid, ReceiveDispatchLineItemRequest> requestLookup = command.Payload.Items
            .ToDictionary(i => i.SourceItemId, i => i);

        foreach (DispatchLineItem lineItem in dispatch.LineItems)
        {
            requestLookup.TryGetValue(lineItem.SourceItemId, out ReceiveDispatchLineItemRequest? request);
            BusinessException.ThrowIfNull(
                request,
                ErrorMessages.Dispatch.ReceiptItemMissing,
                ErrorMessages.Exception.DispatchTitle);

            int totalReceived = request!.ReceivedQuantity + request.DamagedQuantity;
            BusinessException.ThrowIfTrue(
                totalReceived != lineItem.Quantity,
                ErrorMessages.Dispatch.ReceiptQuantityMismatch,
                ErrorMessages.Exception.DispatchTitle);
        }

        List<Guid> sourceItemIds = dispatch.LineItems.Select(li => li.SourceItemId).ToList();
        List<Item> sourceItems = await _itemRepository.GetItemsByFacilityAndIdsAsync(
            dispatch.SourceFacilityId,
            sourceItemIds,
            cancellationToken);

        Dictionary<Guid, Item> itemLookup = sourceItems.ToDictionary(i => i.Id, i => i);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            foreach (DispatchLineItem lineItem in dispatch.LineItems)
            {
                ReceiveDispatchLineItemRequest request = requestLookup[lineItem.SourceItemId];

                itemLookup.TryGetValue(lineItem.SourceItemId, out Item? sourceItem);
                NotFoundException.ThrowIfNull(sourceItem, ErrorMessages.Item.EntityName, lineItem.SourceItemId);

                BusinessException.ThrowIfTrue(
                    sourceItem!.Status is not ItemStatus.InTransit,
                    ErrorMessages.Dispatch.ItemStatusInvalid,
                    ErrorMessages.Exception.DispatchTitle);

                Dictionary<string, string> attributes = sourceItem.DynamicAttributes
                    .ToDictionary(entry => entry.Key, entry => entry.Value);

                string adHocName = sourceItem.Mode is ItemMode.AdHoc ? sourceItem.Name : lineItem.ItemNameSnapshot;
                
                await CreateTargetItemsAsync(
                     dispatch.TargetFacilityId.Value,
                     request,
                     sourceItem,
                     adHocName,
                     attributes,
                     dispatch.Id,
                     cancellationToken);

                sourceItem.Archive(InventoryTransactionType.Archived, dispatch.Id);
                _itemRepository.Update(sourceItem);
            }

            dispatch.MarkCompleted(command.Payload.DeliveryNote);
            _dispatchRepository.Update(dispatch);

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task CreateTargetItemsAsync(
        Guid targetFacilityId,
        ReceiveDispatchLineItemRequest request,
        Item sourceItem,
        string adHocName,
        Dictionary<string, string> attributes,
        Guid dispatchId,
        CancellationToken cancellationToken)
    {
        BusinessException.ThrowIfTrue(
            sourceItem.Mode is ItemMode.Standardized && request.Mode is ItemMode.AdHoc,
            ErrorMessages.Dispatch.StandardizedItemCannotBeAdHoc,
            ErrorMessages.Exception.DispatchTitle);
        
        if (request.Mode is ItemMode.Standardized)
        {
            BusinessException.ThrowIfTrue(
                request.MappedProductId is null || request.MappedProductId == Guid.Empty,
                ErrorMessages.Item.ProductIdRequiredForStandardized,
                ErrorMessages.Exception.InventoryTitle);
        }
        else
        {
            BusinessException.ThrowIfTrue(
                request.MappedProductId is not null,
                ErrorMessages.Item.ProductIdMustBeNullForAdHoc,
                ErrorMessages.Exception.InventoryTitle);
        }
        
        if (request.ReceivedQuantity > 0)
        {
            Item receivedItem = request.Mode is ItemMode.Standardized ?
                Item.CreateFromProduct(
                    request.MappedProductId!.Value,
                    sourceItem.CategoryId,
                    targetFacilityId,
                    request.ReceivedQuantity,
                    sourceItem.UnitOfMeasure,
                    ItemStatus.Available,
                    attributes,
                    InventoryTransactionType.Received,
                    dispatchId) :
                Item.CreateAdHoc(
                    sourceItem.CategoryId,
                    targetFacilityId,
                    adHocName,
                    request.ReceivedQuantity,
                    sourceItem.UnitOfMeasure,
                    ItemStatus.Available,
                    attributes,
                    InventoryTransactionType.Received,
                    dispatchId);

            await _itemRepository.AddAsync(receivedItem, cancellationToken);
        }
        
        if (request.DamagedQuantity > 0)
        {
            Item damagedItem = request.Mode is ItemMode.Standardized ?
                Item.CreateFromProduct(
                    request.MappedProductId!.Value,
                    sourceItem.CategoryId,
                    targetFacilityId,
                    request.DamagedQuantity,
                    sourceItem.UnitOfMeasure,
                    ItemStatus.Damaged,
                    attributes,
                    InventoryTransactionType.Damaged,
                    dispatchId) :
                Item.CreateAdHoc(
                    sourceItem.CategoryId,
                    targetFacilityId,
                    adHocName,
                    request.DamagedQuantity,
                    sourceItem.UnitOfMeasure,
                    ItemStatus.Damaged,
                    attributes,
                    InventoryTransactionType.Damaged,
                    dispatchId);

            await _itemRepository.AddAsync(damagedItem, cancellationToken);
        }
    }
}