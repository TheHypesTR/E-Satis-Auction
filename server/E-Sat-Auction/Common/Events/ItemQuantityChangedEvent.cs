using e_Sat_Auction.Enums;
using MediatR;

namespace e_Sat_Auction.Common.Events;

public sealed record ItemQuantityChangedEvent(
    Guid ItemId,
    Guid FacilityId,
    InventoryTransactionType TransactionType,
    int QuantityChange,
    int PreviousQuantity,
    int NewQuantity,
    Guid? ReferenceId) : INotification;