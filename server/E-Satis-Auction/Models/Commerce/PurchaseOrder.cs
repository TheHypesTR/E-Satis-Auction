using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Common.Events;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Models.Commerce;

public sealed class PurchaseOrder : BaseEntity
{
    public string OrderNumber { get; private set; }
    public string UserId { get; private set; }
    public OrderSource OrderSource { get; private set; }
    public PurchaseOrderStatus Status { get; private set; }
    public ShipmentStatus ShipmentStatus { get; private set; }
    public string Currency { get; private set; }
    public decimal SubtotalAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal ShippingAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public Guid? AppliedCouponCampaignId { get; private set; }
    public Guid? AppliedFreeShippingCampaignId { get; private set; }
    public string? IdempotencyKey { get; private set; }
    public string? ApprovalNote { get; private set; }
    public string? RejectionReason { get; private set; }
    public OrderShippingInfo? ShippingInfo { get; private set; }
    public uint Version { get; private set; }

    private readonly List<PurchaseOrderLine> _lines = [];
    public IReadOnlyCollection<PurchaseOrderLine> Lines => _lines;

    private PurchaseOrder()
    {
        OrderNumber = string.Empty;
        UserId = string.Empty;
        Currency = string.Empty;
        Status = PurchaseOrderStatus.PendingApproval;
        ShipmentStatus = ShipmentStatus.NotShipped;
    }

    public static PurchaseOrder Create(string userId, OrderSource orderSource, string currency)
    {
        return CreateCore(userId, orderSource, currency, PurchaseOrderStatus.PendingApproval, null);
    }

    public static PurchaseOrder CreateForPayment(string userId, OrderSource orderSource, string currency, string idempotencyKey)
    {
        BusinessException.ThrowIfNullOrWhiteSpace(
            idempotencyKey,
            ErrorMessages.Payment.IdempotencyKeyRequired,
            ErrorMessages.Exception.CommerceTitle);

        return CreateCore(userId, orderSource, currency, PurchaseOrderStatus.PaymentPending, idempotencyKey);
    }

    private static PurchaseOrder CreateCore(string userId, OrderSource orderSource, string currency, PurchaseOrderStatus status, string? idempotencyKey)
    {
        BusinessException.ThrowIfNullOrWhiteSpace(
            userId,
            ErrorMessages.PurchaseOrder.UserRequired,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            currency,
            ErrorMessages.PurchaseOrder.CurrencyRequired,
            ErrorMessages.Exception.CommerceTitle);

        PurchaseOrder order = new()
        {
            OrderNumber = GenerateOrderNumber(),
            UserId = userId,
            OrderSource = orderSource,
            Status = status,
            ShipmentStatus = ShipmentStatus.NotShipped,
            Currency = currency.Trim().ToUpperInvariant(),
            IdempotencyKey = string.IsNullOrWhiteSpace(idempotencyKey) ? null : idempotencyKey.Trim()
        };

        order.AddDomainEvent(new PurchaseOrderCreatedDomainEvent(order.Id, order.UserId));
        return order;
    }

    public PurchaseOrderLine AddLine(
        Guid productId,
        Guid productListingId,
        Guid? campaignId,
        string productNameSnapshot,
        string skuSnapshot,
        decimal unitPrice,
        decimal discountedUnitPrice,
        int quantity,
        string currency,
        decimal lineDiscountAmount = 0,
        Guid? appliedCouponCampaignId = null,
        decimal couponDiscountAmount = 0)
    {
        BusinessException.ThrowIfTrue(
            Status is not (PurchaseOrderStatus.PendingApproval or PurchaseOrderStatus.PaymentPending),
            ErrorMessages.PurchaseOrder.CannotMutateSubmittedOrder,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfFalse(
            string.Equals(Currency, currency, StringComparison.OrdinalIgnoreCase),
            ErrorMessages.PurchaseOrder.CurrencyMismatch,
            ErrorMessages.Exception.CommerceTitle);

        PurchaseOrderLine line = PurchaseOrderLine.Create(
            Id,
            productId,
            productListingId,
            campaignId,
            productNameSnapshot,
            skuSnapshot,
            unitPrice,
            discountedUnitPrice,
            quantity,
            Currency,
            lineDiscountAmount,
            appliedCouponCampaignId,
            couponDiscountAmount);

        _lines.Add(line);
        RecalculateTotals();

        return line;
    }

    public void SetIdempotencyKey(string? idempotencyKey)
    {
        IdempotencyKey = string.IsNullOrWhiteSpace(idempotencyKey) ? null : idempotencyKey.Trim();
    }

    public void ApplyOrderPricing(
        decimal subtotalAmount,
        decimal discountAmount,
        decimal shippingAmount,
        decimal totalAmount,
        Guid? appliedCouponCampaignId,
        Guid? appliedFreeShippingCampaignId)
    {
        BusinessException.ThrowIfTrue(
            Status is not (PurchaseOrderStatus.PendingApproval or PurchaseOrderStatus.PaymentPending),
            ErrorMessages.PurchaseOrder.CannotMutateSubmittedOrder,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            subtotalAmount < 0 || discountAmount < 0 || shippingAmount < 0 || totalAmount < 0,
            ErrorMessages.PurchaseOrder.AmountInvalid,
            ErrorMessages.Exception.CommerceTitle);

        SubtotalAmount = subtotalAmount;
        DiscountAmount = discountAmount;
        ShippingAmount = shippingAmount;
        TotalAmount = totalAmount;
        AppliedCouponCampaignId = appliedCouponCampaignId;
        AppliedFreeShippingCampaignId = appliedFreeShippingCampaignId;
    }

    public void MarkPaymentSucceeded()
    {
        BusinessException.ThrowIfTrue(
            Status is not PurchaseOrderStatus.PaymentPending,
            ErrorMessages.PurchaseOrder.StatusMustBePaymentPending,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            _lines.Count is 0,
            ErrorMessages.PurchaseOrder.LinesRequired,
            ErrorMessages.Exception.CommerceTitle);

        Status = PurchaseOrderStatus.PendingApproval;
    }

    public void CancelPaymentPending()
    {
        BusinessException.ThrowIfTrue(
            Status is not PurchaseOrderStatus.PaymentPending,
            ErrorMessages.PurchaseOrder.StatusMustBePaymentPending,
            ErrorMessages.Exception.CommerceTitle);

        Status = PurchaseOrderStatus.Cancelled;
    }

    public void Approve(string approvedByUserId, string? note = null)
    {
        BusinessException.ThrowIfTrue(
            Status is not PurchaseOrderStatus.PendingApproval,
            ErrorMessages.PurchaseOrder.StatusMustBePendingApproval,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            _lines.Count is 0,
            ErrorMessages.PurchaseOrder.LinesRequired,
            ErrorMessages.Exception.CommerceTitle);

        Status = PurchaseOrderStatus.Approved;
        ApprovalNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        AddDomainEvent(new PurchaseOrderApprovedDomainEvent(Id, approvedByUserId));
    }

    public void Reject(string rejectedByUserId, string reason)
    {
        BusinessException.ThrowIfTrue(
            Status is not PurchaseOrderStatus.PendingApproval,
            ErrorMessages.PurchaseOrder.StatusMustBePendingApproval,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            reason,
            ErrorMessages.PurchaseOrder.RejectionReasonRequired,
            ErrorMessages.Exception.CommerceTitle);

        Status = PurchaseOrderStatus.Rejected;
        RejectionReason = reason.Trim();
        AddDomainEvent(new PurchaseOrderRejectedDomainEvent(Id, rejectedByUserId));
    }

    public void MarkShipped(OrderShippingInfo shippingInfo)
    {
        BusinessException.ThrowIfTrue(
            Status is not PurchaseOrderStatus.Approved,
            ErrorMessages.PurchaseOrder.StatusMustBeApproved,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            ShipmentStatus is ShipmentStatus.Shipped,
            ErrorMessages.PurchaseOrder.AlreadyShipped,
            ErrorMessages.Exception.CommerceTitle);

        ShippingInfo = shippingInfo;
        ShipmentStatus = ShipmentStatus.Shipped;
        Status = PurchaseOrderStatus.Shipped;
        AddDomainEvent(new PurchaseOrderShippedDomainEvent(Id, shippingInfo.TrackingNumber));
    }

    private void RecalculateTotals()
    {
        SubtotalAmount = _lines.Sum(line => line.UnitPrice * line.Quantity);
        TotalAmount = _lines.Sum(line => line.DiscountedUnitPrice * line.Quantity) + ShippingAmount;
        DiscountAmount = SubtotalAmount - TotalAmount;
    }

    private static string GenerateOrderNumber()
    {
        string date = DateTimeOffset.UtcNow.ToString("yyMMdd");
        string suffix = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

        return $"ORD-{date}-{suffix}";
    }
}
