using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Commerce;

public sealed record AdminOrderSummaryDto(
    Guid Id,
    string OrderNumber,
    string UserId,
    string UserDisplayName,
    PurchaseOrderStatus Status,
    ShipmentStatus ShipmentStatus,
    OrderSource OrderSource,
    decimal TotalAmount,
    string Currency,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    uint Version);
