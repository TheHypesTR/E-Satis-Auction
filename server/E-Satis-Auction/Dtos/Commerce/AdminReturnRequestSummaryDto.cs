using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Commerce;

public sealed record AdminReturnRequestSummaryDto(
    Guid Id,
    Guid PurchaseOrderId,
    string OrderNumber,
    string UserId,
    string UserDisplayName,
    ReturnRequestStatus Status,
    string Reason,
    DateTime CreatedAt,
    DateTime UpdatedAt);
