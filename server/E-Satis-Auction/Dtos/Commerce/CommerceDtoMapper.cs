using E_Satis_Auction.Models.Commerce;

namespace E_Satis_Auction.Dtos.Commerce;

public static class CommerceDtoMapper
{
    public static ProductListingSummaryDto ToProductListingSummaryDto(
        ProductListing listing,
        ProductListingProductEnrichmentDto product,
        string sourceFacilityName)
    {
        return new ProductListingSummaryDto(
            listing.Id,
            listing.ProductId,
            product.Name,
            product.Sku,
            listing.SourceFacilityId,
            sourceFacilityName,
            listing.SalePrice,
            listing.Currency,
            listing.Status,
            listing.ActiveFrom,
            listing.ActiveUntil,
            listing.Version);
    }

    public static ProductListingDetailDto ToProductListingDetailDto(
        ProductListing listing,
        ProductListingProductEnrichmentDto product,
        string sourceFacilityName,
        int availableStockQuantity)
    {
        return new ProductListingDetailDto(
            listing.Id,
            listing.ProductId,
            product.Name,
            product.Sku,
            product.CategoryId,
            listing.SourceFacilityId,
            sourceFacilityName,
            listing.SalePrice,
            listing.Currency,
            listing.Status,
            availableStockQuantity,
            listing.ActiveFrom,
            listing.ActiveUntil,
            listing.CreatedAt,
            listing.UpdatedAt,
            listing.Version);
    }

    public static AdminProductListingSummaryDto ToAdminProductListingSummaryDto(
        ProductListing listing,
        ProductListingProductEnrichmentDto product,
        string sourceFacilityName)
    {
        return new AdminProductListingSummaryDto(
            listing.Id,
            listing.ProductId,
            product.Name,
            product.Sku,
            listing.SourceFacilityId,
            sourceFacilityName,
            listing.SalePrice,
            listing.Currency,
            listing.Status,
            listing.CreatedAt,
            listing.UpdatedAt,
            listing.Version);
    }

    public static AdminProductListingDetailDto ToAdminProductListingDetailDto(
        ProductListing listing,
        ProductListingProductEnrichmentDto product,
        string sourceFacilityName,
        int availableStockQuantity)
    {
        return new AdminProductListingDetailDto(
            listing.Id,
            listing.ProductId,
            product.Name,
            product.Sku,
            product.CategoryId,
            product.IsActive,
            listing.SourceFacilityId,
            sourceFacilityName,
            listing.SalePrice,
            listing.Currency,
            listing.Status,
            availableStockQuantity,
            listing.ActiveFrom,
            listing.ActiveUntil,
            listing.CreatedAt,
            listing.UpdatedAt,
            listing.Version);
    }

    public static OrderSummaryDto ToOrderSummaryDto(PurchaseOrder order)
    {
        return new OrderSummaryDto(
            order.Id,
            order.OrderNumber,
            order.Status,
            order.ShipmentStatus,
            order.OrderSource,
            order.TotalAmount,
            order.Currency,
            order.CreatedAt,
            order.UpdatedAt,
            order.Version);
    }

    public static AdminOrderSummaryDto ToAdminOrderSummaryDto(PurchaseOrder order, string userDisplayName)
    {
        return new AdminOrderSummaryDto(
            order.Id,
            order.OrderNumber,
            order.UserId,
            userDisplayName,
            order.Status,
            order.ShipmentStatus,
            order.OrderSource,
            order.TotalAmount,
            order.Currency,
            order.CreatedAt,
            order.UpdatedAt,
            order.Version);
    }

    public static OrderDetailDto ToOrderDetailDto(
        PurchaseOrder order,
        IReadOnlyCollection<ReturnRequestSummaryDto>? returnRequests = null)
    {
        return new OrderDetailDto(
            order.Id,
            order.OrderNumber,
            order.Status,
            order.ShipmentStatus,
            order.OrderSource,
            order.SubtotalAmount,
            order.DiscountAmount,
            order.ShippingAmount,
            order.TotalAmount,
            order.AppliedCouponCampaignId,
            order.AppliedFreeShippingCampaignId,
            order.Currency,
            order.ApprovalNote,
            order.RejectionReason,
            ToShippingInfoDto(order.ShippingInfo),
            order.Lines.Select(ToOrderLineDto).ToList(),
            returnRequests ?? [],
            order.CreatedAt,
            order.UpdatedAt,
            order.Version);
    }

    public static AdminOrderDetailDto ToAdminOrderDetailDto(
        PurchaseOrder order,
        string userDisplayName,
        IReadOnlyCollection<ReturnRequestSummaryDto>? returnRequests = null)
    {
        return new AdminOrderDetailDto(
            order.Id,
            order.OrderNumber,
            order.UserId,
            userDisplayName,
            order.Status,
            order.ShipmentStatus,
            order.OrderSource,
            order.SubtotalAmount,
            order.DiscountAmount,
            order.ShippingAmount,
            order.TotalAmount,
            order.AppliedCouponCampaignId,
            order.AppliedFreeShippingCampaignId,
            order.Currency,
            order.ApprovalNote,
            order.RejectionReason,
            ToShippingInfoDto(order.ShippingInfo),
            order.Lines.Select(ToOrderLineDto).ToList(),
            returnRequests ?? [],
            order.CreatedAt,
            order.UpdatedAt,
            order.Version);
    }

    public static ReturnRequestSummaryDto ToReturnRequestSummaryDto(ReturnRequest returnRequest)
    {
        return new ReturnRequestSummaryDto(
            returnRequest.Id,
            returnRequest.PurchaseOrderId,
            returnRequest.UserId,
            returnRequest.Status,
            returnRequest.Reason,
            returnRequest.CreatedAt,
            returnRequest.UpdatedAt);
    }

    public static AdminReturnRequestSummaryDto ToAdminReturnRequestSummaryDto(
        ReturnRequest returnRequest,
        string orderNumber,
        string userDisplayName)
    {
        return new AdminReturnRequestSummaryDto(
            returnRequest.Id,
            returnRequest.PurchaseOrderId,
            orderNumber,
            returnRequest.UserId,
            userDisplayName,
            returnRequest.Status,
            returnRequest.Reason,
            returnRequest.CreatedAt,
            returnRequest.UpdatedAt);
    }

    public static ReturnRequestDetailDto ToReturnRequestDetailDto(ReturnRequest returnRequest)
    {
        return new ReturnRequestDetailDto(
            returnRequest.Id,
            returnRequest.PurchaseOrderId,
            returnRequest.UserId,
            returnRequest.Status,
            returnRequest.Reason,
            returnRequest.ResolutionNote,
            returnRequest.ReceivedAt,
            returnRequest.ReceivedByUserId,
            returnRequest.ReceiveNote,
            returnRequest.Lines.Select(ToReturnRequestLineDto).ToList(),
            returnRequest.CreatedAt,
            returnRequest.UpdatedAt);
    }

    public static CartDto ToCartDto(ShoppingCart cart, CartPricePreviewDto? preview = null)
    {
        return new CartDto(
            cart.Id,
            cart.ProductListingId,
            cart.Quantity,
            preview?.AppliedCouponCampaignId ?? cart.AppliedCouponCampaignId,
            preview?.AppliedFreeShippingCampaignId,
            preview?.SubtotalAmount ?? cart.PreviewSubtotalAmount,
            preview?.DiscountAmount ?? cart.PreviewDiscountAmount,
            preview?.ShippingAmount ?? cart.PreviewShippingAmount,
            preview?.TotalAmount ?? cart.PreviewTotalAmount,
            preview?.Currency ?? cart.Currency,
            cart.Status,
            cart.Version);
    }

    public static PaymentAttemptDto ToPaymentAttemptDto(PaymentAttempt payment)
    {
        return new PaymentAttemptDto(
            payment.Id,
            payment.PurchaseOrderId,
            payment.Amount,
            payment.Currency,
            payment.Status,
            payment.ExpiresAt,
            payment.FailureReason,
            payment.CreatedAt,
            payment.UpdatedAt,
            payment.Version);
    }

    public static CampaignDto ToCampaignDto(Campaign campaign)
    {
        return new CampaignDto(
            campaign.Id,
            campaign.Name,
            campaign.Description,
            campaign.CouponCode,
            campaign.Scope,
            campaign.DiscountType,
            campaign.DiscountValue,
            campaign.MinimumOrderAmount,
            campaign.ProductListingId,
            campaign.CategoryId,
            campaign.Currency,
            campaign.Status,
            campaign.StartsAt,
            campaign.EndsAt,
            campaign.Version);
    }

    public static UserSaleRequestDto ToUserSaleRequestDto(UserSaleRequest request)
    {
        return new UserSaleRequestDto(
            request.Id,
            request.UserId,
            request.Title,
            request.Description,
            request.CategoryId,
            request.UserEstimatedValue,
            request.AcquisitionPrice,
            request.TargetResalePrice,
            request.ExpectedProfit,
            request.Status,
            request.AdminNote,
            request.CreatedAt,
            request.UpdatedAt,
            request.Version);
    }

    public static PartSaleOperationDto ToPartSaleOperationDto(PartSaleOperation operation)
    {
        return new PartSaleOperationDto(
            operation.Id,
            operation.SourceItemId,
            operation.CreatedPartItemId,
            operation.ProductId,
            operation.FacilityId,
            operation.Quantity,
            operation.UnitOfMeasure,
            operation.Notes,
            operation.Status,
            operation.CreatedAt);
    }

    private static OrderLineDto ToOrderLineDto(PurchaseOrderLine line)
    {
        return new OrderLineDto(
            line.Id,
            line.ProductId,
            line.ProductListingId,
            line.CampaignId,
            line.ProductNameSnapshot,
            line.SkuSnapshot,
            line.UnitPrice,
            line.DiscountedUnitPrice,
            line.Quantity,
            line.Currency,
            line.Allocations.Select(ToOrderLineAllocationDto).ToList());
    }

    private static OrderLineAllocationDto ToOrderLineAllocationDto(PurchaseOrderLineAllocation allocation)
    {
        return new OrderLineAllocationDto(allocation.Id, allocation.OriginalItemId, allocation.ReservedItemId, allocation.Quantity);
    }

    private static ReturnRequestLineDto ToReturnRequestLineDto(ReturnRequestLine line)
    {
        return new ReturnRequestLineDto(
            line.Id,
            line.PurchaseOrderLineId,
            line.Quantity,
            line.Reason,
            line.ReceivedQuantity,
            line.RestockedQuantity,
            line.ReceiveNote);
    }

    private static OrderShippingInfoDto? ToShippingInfoDto(OrderShippingInfo? shippingInfo)
    {
        return shippingInfo is null
            ? null
            : new OrderShippingInfoDto(
                shippingInfo.CarrierName,
                shippingInfo.TrackingNumber,
                shippingInfo.TrackingUrl,
                shippingInfo.Notes,
                shippingInfo.ShippedAt);
    }
}
