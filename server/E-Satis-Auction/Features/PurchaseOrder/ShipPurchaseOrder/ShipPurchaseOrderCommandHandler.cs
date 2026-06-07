using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.PurchaseOrder.ShipPurchaseOrder;

using ItemEntity = Models.Items.Item;
using OrderShippingInfoEntity = Models.Commerce.OrderShippingInfo;
using PurchaseOrderEntity = Models.Commerce.PurchaseOrder;

public sealed class ShipPurchaseOrderCommandHandler : ICommandHandler<ShipPurchaseOrderCommand>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ShipPurchaseOrderCommandHandler(
        IPurchaseOrderRepository purchaseOrderRepository,
        IItemRepository itemRepository,
        IUnitOfWork unitOfWork)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _itemRepository = itemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ShipPurchaseOrderCommand command, CancellationToken cancellationToken)
    {
        PurchaseOrderEntity? order = await _purchaseOrderRepository.GetByIdWithDetailsAsync(command.PurchaseOrderId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(order, ErrorMessages.PurchaseOrder.EntityName, command.PurchaseOrderId);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            OrderShippingInfoEntity shippingInfo = OrderShippingInfoEntity.Create(
                command.Payload.CarrierName,
                command.Payload.TrackingNumber,
                command.Payload.ShippedAt ?? DateTimeOffset.UtcNow,
                command.Payload.TrackingUrl,
                command.Payload.Notes);

            order!.MarkShipped(shippingInfo);
            await ArchiveReservedStockAsync(order, cancellationToken);
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

    private async Task ArchiveReservedStockAsync(PurchaseOrderEntity order, CancellationToken cancellationToken)
    {
        List<Guid> reservedItemIds = order.Lines
            .SelectMany(line => line.Allocations)
            .Select(allocation => allocation.ReservedItemId)
            .Distinct()
            .ToList();

        if (reservedItemIds.Count is 0)
        {
            return;
        }

        List<ItemEntity> reservedItems = await _itemRepository.GetItemsByIdsAsync(reservedItemIds, enableTracking: true, cancellationToken);
        Dictionary<Guid, ItemEntity> itemLookup = reservedItems.ToDictionary(item => item.Id, item => item);

        foreach (Guid reservedItemId in reservedItemIds)
        {
            itemLookup.TryGetValue(reservedItemId, out ItemEntity? reservedItem);
            NotFoundException.ThrowIfNull(reservedItem, ErrorMessages.Item.EntityName, reservedItemId);

            BusinessException.ThrowIfTrue(
                reservedItem!.Status is not ItemStatus.Reserved,
                ErrorMessages.PurchaseOrder.InvalidReservedInventoryState,
                ErrorMessages.Exception.CommerceTitle);

            reservedItem.Archive(InventoryTransactionType.PurchaseShipped, order.Id);
            _itemRepository.Update(reservedItem);
        }
    }
}
