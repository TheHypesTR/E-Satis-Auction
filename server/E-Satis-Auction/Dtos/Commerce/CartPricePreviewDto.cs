namespace E_Satis_Auction.Dtos.Commerce;

public sealed record CartPricePreviewDto(
    Guid ProductListingId,
    int Quantity,
    decimal OriginalUnitPrice,
    decimal DiscountedUnitPrice,
    decimal LineDiscountAmount,
    decimal CouponDiscountAmount,
    decimal SubtotalAmount,
    decimal DiscountAmount,
    decimal ShippingAmount,
    decimal TotalAmount,
    string Currency,
    Guid? AppliedLineCampaignId,
    Guid? AppliedCouponCampaignId,
    Guid? AppliedFreeShippingCampaignId);
