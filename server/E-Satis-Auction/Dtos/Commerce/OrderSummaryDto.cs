using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Commerce;

public sealed record OrderSummaryDto(
    Guid Id,
    string OrderNumber,
    PurchaseOrderStatus Status,
    ShipmentStatus ShipmentStatus,
    OrderSource OrderSource,
    decimal TotalAmount,
    string Currency,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    uint Version);
