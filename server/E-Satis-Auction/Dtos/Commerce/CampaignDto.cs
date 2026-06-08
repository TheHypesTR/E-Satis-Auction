using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Commerce;

public sealed record CampaignDto(
    Guid Id,
    string Name,
    string? Description,
    string? CouponCode,
    CampaignScope Scope,
    DiscountType DiscountType,
    decimal DiscountValue,
    decimal? MinimumOrderAmount,
    Guid? ProductListingId,
    Guid? CategoryId,
    string? Currency,
    CampaignStatus Status,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    uint Version);
