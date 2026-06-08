using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Commerce;

public sealed record UserSaleRequestDto(
    Guid Id,
    string UserId,
    string Title,
    string Description,
    Guid CategoryId,
    decimal UserEstimatedValue,
    decimal? AcquisitionPrice,
    decimal? TargetResalePrice,
    decimal? ExpectedProfit,
    UserSaleRequestStatus Status,
    string? AdminNote,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    uint Version);
