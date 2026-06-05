using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.PurchaseOrder.RejectPurchaseOrder;

using ItemEntity = Models.Items.Item;
using PurchaseOrderEntity = Models.Commerce.PurchaseOrder;

public sealed class RejectPurchaseOrderCommandHandler : ICommandHandler<RejectPurchaseOrderCommand>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public RejectPurchaseOrderCommandHandler(
        IPurchaseOrderRepository purchaseOrderRepository,
        IItemRepository itemRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _itemRepository = itemRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task Handle(RejectPurchaseOrderCommand command, CancellationToken cancellationToken)
    {
        PurchaseOrderEntity? order = await _purchaseOrderRepository.GetByIdWithDetailsAsync(command.PurchaseOrderId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(order, ErrorMessages.PurchaseOrder.EntityName, command.PurchaseOrderId);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await ReleaseReservedStockAsync(order!, cancellationToken);
            order!.Reject(_currentUserService.UserId, command.Payload.Reason);
            _purchaseOrderRepository.Update(order);

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task ReleaseReservedStockAsync(PurchaseOrderEntity order, CancellationToken cancellationToken)
    {
        List<Models.Commerce.PurchaseOrderLineAllocation> allocations = order.Lines
            .SelectMany(line => line.Allocations)
            .ToList();

        if (allocations.Count is 0)
        {
            return;
        }

        List<Guid> itemIds = allocations
            .SelectMany(allocation => new[] { allocation.OriginalItemId, allocation.ReservedItemId })
            .Distinct()
            .ToList();

        List<ItemEntity> items = await _itemRepository.GetItemsByIdsAsync(itemIds, enableTracking: true, cancellationToken);
        Dictionary<Guid, ItemEntity> itemLookup = items.ToDictionary(item => item.Id, item => item);

        foreach (Models.Commerce.PurchaseOrderLineAllocation allocation in allocations)
        {
            itemLookup.TryGetValue(allocation.OriginalItemId, out ItemEntity? originalItem);
            itemLookup.TryGetValue(allocation.ReservedItemId, out ItemEntity? reservedItem);

            NotFoundException.ThrowIfNull(originalItem, ErrorMessages.Item.EntityName, allocation.OriginalItemId);
            NotFoundException.ThrowIfNull(reservedItem, ErrorMessages.Item.EntityName, allocation.ReservedItemId);

            BusinessException.ThrowIfTrue(
                reservedItem!.Status is not ItemStatus.Reserved,
                ErrorMessages.PurchaseOrder.InvalidReservedInventoryState,
                ErrorMessages.Exception.CommerceTitle);

            originalItem!.IncreaseQuantity(allocation.Quantity, InventoryTransactionType.PurchaseReleased, order.Id);
            reservedItem.Archive(InventoryTransactionType.PurchaseReleased, order.Id);

            _itemRepository.Update(originalItem);
            _itemRepository.Update(reservedItem);
        }
    }
}
