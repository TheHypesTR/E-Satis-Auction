using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Commerce;

public sealed record AdminOrderDetailDto(
    Guid Id,
    string OrderNumber,
    string UserId,
    string UserDisplayName,
    PurchaseOrderStatus Status,
    ShipmentStatus ShipmentStatus,
    OrderSource OrderSource,
    decimal SubtotalAmount,
    decimal DiscountAmount,
    decimal ShippingAmount,
    decimal TotalAmount,
    Guid? AppliedCouponCampaignId,
    Guid? AppliedFreeShippingCampaignId,
    string Currency,
    string? ApprovalNote,
    string? RejectionReason,
    OrderShippingInfoDto? ShippingInfo,
    IReadOnlyCollection<OrderLineDto> Lines,
    IReadOnlyCollection<ReturnRequestSummaryDto> ReturnRequests,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    uint Version);
