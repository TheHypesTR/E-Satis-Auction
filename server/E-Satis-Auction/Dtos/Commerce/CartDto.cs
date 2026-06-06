using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Commerce;

public sealed record CartDto(
    Guid Id,
    Guid ProductListingId,
    int Quantity,
    Guid? AppliedCouponCampaignId,
    Guid? AppliedFreeShippingCampaignId,
    decimal PreviewSubtotalAmount,
    decimal PreviewDiscountAmount,
    decimal PreviewShippingAmount,
    decimal PreviewTotalAmount,
    string Currency,
    CartStatus Status,
    uint Version);
