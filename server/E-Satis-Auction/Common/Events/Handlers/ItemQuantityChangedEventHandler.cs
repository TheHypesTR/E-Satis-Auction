using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.InventoryTransactions;
using MediatR;

namespace E_Satis_Auction.Common.Events.Handlers;

public sealed class ItemQuantityChangedEventHandler : INotificationHandler<ItemQuantityChangedEvent>
{
    private readonly IInventoryTransactionRepository _inventoryTransactionRepository;
    private readonly ICurrentUserService _currentUserService;

    public ItemQuantityChangedEventHandler(
        IInventoryTransactionRepository inventoryTransactionRepository,
        ICurrentUserService currentUserService)
    {
        _inventoryTransactionRepository = inventoryTransactionRepository;
        _currentUserService = currentUserService;
    }

    public async Task Handle(ItemQuantityChangedEvent notification, CancellationToken cancellationToken)
    {
        string createdBy = string.IsNullOrWhiteSpace(_currentUserService.UserId) ? SystemConstants.SystemUser : _currentUserService.UserId;
        InventoryTransaction transaction = InventoryTransaction.Create(
            notification.ItemId,
            notification.FacilityId,
            notification.TransactionType,
            notification.QuantityChange,
            notification.PreviousQuantity,
            notification.NewQuantity,
            notification.ReferenceId,
            createdBy);

        await _inventoryTransactionRepository.AddAsync(transaction, cancellationToken);
    }
}