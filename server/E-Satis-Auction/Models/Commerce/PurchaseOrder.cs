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
    public decimal TotalAmount { get; private set; }
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
            Status = PurchaseOrderStatus.PendingApproval,
            ShipmentStatus = ShipmentStatus.NotShipped,
            Currency = currency.Trim().ToUpperInvariant()
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
        string currency)
    {
        BusinessException.ThrowIfTrue(
            Status is not PurchaseOrderStatus.PendingApproval,
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
            Currency);

        _lines.Add(line);
        RecalculateTotals();

        return line;
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
        TotalAmount = _lines.Sum(line => line.DiscountedUnitPrice * line.Quantity);
        DiscountAmount = SubtotalAmount - TotalAmount;
    }

    private static string GenerateOrderNumber()
    {
        string date = DateTimeOffset.UtcNow.ToString("yyMMdd");
        string suffix = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

        return $"ORD-{date}-{suffix}";
    }
}
