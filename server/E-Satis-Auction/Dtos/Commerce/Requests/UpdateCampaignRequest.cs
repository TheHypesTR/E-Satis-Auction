using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Commerce.Requests;

public sealed record UpdateCampaignRequest(
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
    DateTimeOffset? StartsAt,
    DateTimeOffset? EndsAt);
