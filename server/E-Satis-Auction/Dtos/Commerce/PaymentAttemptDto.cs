using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Commerce;

public sealed record PaymentAttemptDto(
    Guid Id,
    Guid PurchaseOrderId,
    decimal Amount,
    string Currency,
    PaymentStatus Status,
    DateTimeOffset ExpiresAt,
    string? FailureReason,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    uint Version);
