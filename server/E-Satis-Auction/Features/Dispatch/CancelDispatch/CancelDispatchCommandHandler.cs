using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Dispatch.CancelDispatch;

using Models.Dispatches;
using Models.Items;

public sealed class CancelDispatchCommandHandler : ICommandHandler<CancelDispatchCommand>
{
    private readonly IDispatchRepository _dispatchRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CancelDispatchCommandHandler(
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

    public async Task Handle(CancelDispatchCommand command, CancellationToken cancellationToken)
    {
        Dispatch? dispatch = await _dispatchRepository.GetByIdWithLineItemsAsync(command.DispatchId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(dispatch, ErrorMessages.Dispatch.EntityName, command.DispatchId);

        bool hasFacilityAccess = await _currentUserService.HasFacilityAccess(dispatch!.SourceFacilityId, cancellationToken);
        ForbiddenAccessException.ThrowIfFalse(
            hasFacilityAccess,
            ErrorMessages.Facility.UnauthorizedFacilityAccess,
            ErrorMessages.Exception.UnauthorizedAccess);
        
        if (dispatch.Status is DispatchStatus.Cancelled)
        {
            return;
        }
        
        List<Guid> allItemIds = dispatch.LineItems
            .SelectMany(li => new[] { li.SourceItemId, li.OriginalItemId })
            .Distinct()
            .ToList();

        List<Item> allItems = await _itemRepository.GetItemsByFacilityAndIdsAsync(
            dispatch.SourceFacilityId,
            allItemIds,
            cancellationToken);

        Dictionary<Guid, Item> itemLookup = allItems.ToDictionary(i => i.Id, i => i);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            dispatch.MarkCancelled(command.Payload.CancellationNote);
            _dispatchRepository.Update(dispatch);

            foreach (DispatchLineItem lineItem in dispatch.LineItems)
            {
                itemLookup.TryGetValue(lineItem.SourceItemId, out Item? reservedItem);
                itemLookup.TryGetValue(lineItem.OriginalItemId, out Item? originalItem);
                NotFoundException.ThrowIfNull(reservedItem, ErrorMessages.Item.EntityName, lineItem.SourceItemId);

                BusinessException.ThrowIfTrue(
                    reservedItem!.Status is not ItemStatus.Reserved,
                    ErrorMessages.Dispatch.ItemStatusInvalid,
                    ErrorMessages.Exception.DispatchTitle);

                if (originalItem is not null && originalItem.Status is not ItemStatus.Archived)
                {
                    originalItem.IncreaseQuantity(lineItem.Quantity, InventoryTransactionType.Cancelled, dispatch.Id);
                    _itemRepository.Update(originalItem);
                    reservedItem.Archive(InventoryTransactionType.Archived, dispatch.Id);
                }
                else
                {
                    reservedItem.UpdateStatus(ItemStatus.Available);
                }

                _itemRepository.Update(reservedItem);
            }

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}