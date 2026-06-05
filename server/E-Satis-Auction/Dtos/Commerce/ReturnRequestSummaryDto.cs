using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Commerce;

public sealed record ReturnRequestSummaryDto(
    Guid Id,
    Guid PurchaseOrderId,
    string UserId,
    ReturnRequestStatus Status,
    string Reason,
    DateTime CreatedAt,
    DateTime UpdatedAt);
