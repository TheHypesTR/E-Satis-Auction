using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Dispatch.CompleteAddressDispatch;

using Models.Dispatches;
using Models.Items;

public sealed class CompleteAddressDispatchCommandHandler : ICommandHandler<CompleteAddressDispatchCommand>
{
    private readonly IDispatchRepository _dispatchRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CompleteAddressDispatchCommandHandler(
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

    public async Task Handle(CompleteAddressDispatchCommand command, CancellationToken cancellationToken)
    {
        Dispatch? dispatch = await _dispatchRepository.GetByIdWithLineItemsAsync(command.DispatchId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(dispatch, ErrorMessages.Dispatch.EntityName, command.DispatchId);

        BusinessException.ThrowIfTrue(
            dispatch!.TargetAddressId is null,
            ErrorMessages.Dispatch.TargetAddressRequiredForDelivery,
            ErrorMessages.Exception.DispatchTitle);

        BusinessException.ThrowIfTrue(
            dispatch.TargetFacilityId is not null,
            ErrorMessages.Dispatch.TargetFacilityMustBeNullForDelivery,
            ErrorMessages.Exception.DispatchTitle);

        bool hasFacilityAccess = await _currentUserService.HasFacilityAccess(dispatch.SourceFacilityId, cancellationToken);
        ForbiddenAccessException.ThrowIfFalse(
            hasFacilityAccess,
            ErrorMessages.Facility.UnauthorizedFacilityAccess,
            ErrorMessages.Exception.UnauthorizedAccess);

        BusinessException.ThrowIfTrue(
            dispatch.Status is not DispatchStatus.InTransit,
            ErrorMessages.Dispatch.StatusNotInTransit,
            ErrorMessages.Exception.DispatchTitle);

        List<Guid> sourceItemIds = dispatch.LineItems
            .Select(li => li.SourceItemId)
            .Distinct()
            .ToList();

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
                itemLookup.TryGetValue(lineItem.SourceItemId, out Item? item);
                NotFoundException.ThrowIfNull(item, ErrorMessages.Item.EntityName, lineItem.SourceItemId);

                BusinessException.ThrowIfTrue(
                    item!.Status is not ItemStatus.InTransit,
                    ErrorMessages.Dispatch.ItemStatusInvalid,
                    ErrorMessages.Exception.DispatchTitle);

                item.Archive(InventoryTransactionType.Archived, dispatch.Id);
                _itemRepository.Update(item);
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
}