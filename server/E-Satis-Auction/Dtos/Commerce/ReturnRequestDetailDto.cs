using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Commerce;

public sealed record ReturnRequestDetailDto(
    Guid Id,
    Guid PurchaseOrderId,
    string UserId,
    ReturnRequestStatus Status,
    string Reason,
    string? ResolutionNote,
    DateTime? ReceivedAt,
    string? ReceivedByUserId,
    string? ReceiveNote,
    IReadOnlyCollection<ReturnRequestLineDto> Lines,
    DateTime CreatedAt,
    DateTime UpdatedAt);
