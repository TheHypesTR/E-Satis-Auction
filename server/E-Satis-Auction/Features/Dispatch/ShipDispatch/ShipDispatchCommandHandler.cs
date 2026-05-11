using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Dispatch.ShipDispatch;

using Models.Dispatches;
using Models.Items;

public class ShipDispatchCommandHandler : ICommandHandler<ShipDispatchCommand>
{
    private readonly IDispatchRepository _dispatchRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ShipDispatchCommandHandler(
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

    public async Task Handle(ShipDispatchCommand command, CancellationToken cancellationToken)
    {
        Dispatch? dispatch = await _dispatchRepository.GetByIdWithLineItemsAsync(command.DispatchId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(dispatch, ErrorMessages.Dispatch.EntityName, command.DispatchId);

        bool hasFacilityAccess = await _currentUserService.HasFacilityAccess(dispatch!.SourceFacilityId, cancellationToken);
        ForbiddenAccessException.ThrowIfFalse(
            hasFacilityAccess,
            ErrorMessages.Facility.UnauthorizedFacilityAccess,
            ErrorMessages.Exception.UnauthorizedAccess);

        BusinessException.ThrowIfTrue(
            dispatch.Status is not DispatchStatus.Pending,
            ErrorMessages.Dispatch.StatusNotPending,
            ErrorMessages.Exception.DispatchTitle);

        List<Guid> reservedItemIds = dispatch.LineItems
            .Select(li => li.SourceItemId)
            .Distinct()
            .ToList();

        List<Item> reservedItems = await _itemRepository.GetItemsByFacilityAndIdsAsync(
            dispatch.SourceFacilityId, 
            reservedItemIds, 
            cancellationToken);

        Dictionary<Guid, Item> itemLookup = reservedItems.ToDictionary(i => i.Id, i => i);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            dispatch.MarkInTransit(DateTimeOffset.UtcNow);
            _dispatchRepository.Update(dispatch);

            foreach (DispatchLineItem lineItem in dispatch.LineItems)
            {
                itemLookup.TryGetValue(lineItem.SourceItemId, out Item? item);
                NotFoundException.ThrowIfNull(item, ErrorMessages.Item.EntityName, lineItem.SourceItemId);

                BusinessException.ThrowIfTrue(
                    item!.Status is not ItemStatus.Reserved,
                    ErrorMessages.Dispatch.ItemStatusInvalid,
                    ErrorMessages.Exception.DispatchTitle);

                item.UpdateStatus(ItemStatus.InTransit);
                _itemRepository.Update(item);
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