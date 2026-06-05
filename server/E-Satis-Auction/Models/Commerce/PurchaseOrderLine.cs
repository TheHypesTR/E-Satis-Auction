using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Common.Exceptions;

namespace E_Satis_Auction.Models.Commerce;

public sealed class PurchaseOrderLine : BaseEntity
{
    public Guid PurchaseOrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductListingId { get; private set; }
    public Guid? CampaignId { get; private set; }
    public string ProductNameSnapshot { get; private set; }
    public string SkuSnapshot { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountedUnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public string Currency { get; private set; }

    private readonly List<PurchaseOrderLineAllocation> _allocations = [];
    public IReadOnlyCollection<PurchaseOrderLineAllocation> Allocations => _allocations;

    private PurchaseOrderLine()
    {
        ProductNameSnapshot = string.Empty;
        SkuSnapshot = string.Empty;
        Currency = string.Empty;
    }

    public static PurchaseOrderLine Create(
        Guid purchaseOrderId,
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
        Validate(
            purchaseOrderId,
            productId,
            productListingId,
            productNameSnapshot,
            skuSnapshot,
            unitPrice,
            discountedUnitPrice,
            quantity,
            currency);

        return new PurchaseOrderLine
        {
            PurchaseOrderId = purchaseOrderId,
            ProductId = productId,
            ProductListingId = productListingId,
            CampaignId = campaignId,
            ProductNameSnapshot = productNameSnapshot.Trim(),
            SkuSnapshot = skuSnapshot.Trim(),
            UnitPrice = unitPrice,
            DiscountedUnitPrice = discountedUnitPrice,
            Quantity = quantity,
            Currency = currency.Trim().ToUpperInvariant()
        };
    }

    public PurchaseOrderLineAllocation AddAllocation(Guid originalItemId, Guid reservedItemId, int quantity)
    {
        PurchaseOrderLineAllocation allocation = PurchaseOrderLineAllocation.Create(Id, originalItemId, reservedItemId, quantity);
        _allocations.Add(allocation);

        return allocation;
    }

    private static void Validate(
        Guid purchaseOrderId,
        Guid productId,
        Guid productListingId,
        string productNameSnapshot,
        string skuSnapshot,
        decimal unitPrice,
        decimal discountedUnitPrice,
        int quantity,
        string currency)
    {
        BusinessException.ThrowIfTrue(
            purchaseOrderId == Guid.Empty,
            ErrorMessages.PurchaseOrder.EntityName,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            productId == Guid.Empty,
            ErrorMessages.PurchaseOrder.ProductRequired,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            productListingId == Guid.Empty,
            ErrorMessages.PurchaseOrder.ProductListingRequired,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            productNameSnapshot,
            ErrorMessages.PurchaseOrder.ProductNameSnapshotRequired,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            skuSnapshot,
            ErrorMessages.PurchaseOrder.SkuSnapshotRequired,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            unitPrice <= 0,
            ErrorMessages.PurchaseOrder.UnitPriceMustBePositive,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            discountedUnitPrice < 0 || discountedUnitPrice > unitPrice,
            ErrorMessages.PurchaseOrder.DiscountedPriceInvalid,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            quantity <= 0,
            ErrorMessages.PurchaseOrder.QuantityMustBePositive,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            currency,
            ErrorMessages.PurchaseOrder.CurrencyRequired,
            ErrorMessages.Exception.CommerceTitle);
    }
}
