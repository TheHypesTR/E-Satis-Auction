using e_Sat_Auction.Enums;

namespace e_Sat_Auction.Dtos.InventoryTransaction;

public sealed record InventoryTransactionDto(
    Guid Id,
    Guid ItemId,
    string ItemName,
    UnitOfMeasure UnitOfMeasure,
    Guid FacilityId,
    string FacilityName,
    InventoryTransactionType TransactionType,
    int QuantityChange,
    int PreviousQuantity,
    int NewQuantity,
    Guid? ReferenceId,
    string? ReferenceTrackingNumber,
    string CreatedByUserId,
    string CreatedByUserName,
    DateTime CreatedAt);