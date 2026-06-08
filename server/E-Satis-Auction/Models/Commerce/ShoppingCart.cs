using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Models.Commerce;

public sealed class ShoppingCart : BaseEntity
{
    public string UserId { get; private set; }
    public Guid ProductListingId { get; private set; }
    public int Quantity { get; private set; }
    public Guid? AppliedCouponCampaignId { get; private set; }
    public decimal PreviewSubtotalAmount { get; private set; }
    public decimal PreviewDiscountAmount { get; private set; }
    public decimal PreviewShippingAmount { get; private set; }
    public decimal PreviewTotalAmount { get; private set; }
    public string Currency { get; private set; }
    public CartStatus Status { get; private set; }
    public uint Version { get; private set; }

    private ShoppingCart()
    {
        UserId = string.Empty;
        Currency = string.Empty;
        Status = CartStatus.Active;
    }

    public static ShoppingCart Create(string userId, Guid productListingId, int quantity)
    {
        Validate(userId, productListingId, quantity);

        return new ShoppingCart
        {
            UserId = userId,
            ProductListingId = productListingId,
            Quantity = quantity,
            Status = CartStatus.Active
        };
    }

    public void ReplaceListing(Guid productListingId, int quantity)
    {
        BusinessException.ThrowIfTrue(
            Status is not CartStatus.Active,
            ErrorMessages.Cart.NotActive,
            ErrorMessages.Exception.CommerceTitle);

        Validate(UserId, productListingId, quantity);
        ProductListingId = productListingId;
        Quantity = quantity;
        AppliedCouponCampaignId = null;
    }

    public void ApplyCoupon(Guid campaignId)
    {
        BusinessException.ThrowIfTrue(
            campaignId == Guid.Empty,
            ErrorMessages.Campaign.EntityName,
            ErrorMessages.Exception.CommerceTitle);

        AppliedCouponCampaignId = campaignId;
    }

    public void RemoveCoupon()
    {
        AppliedCouponCampaignId = null;
    }

    public void UpdatePreview(decimal subtotal, decimal discount, decimal shipping, decimal total, string currency)
    {
        BusinessException.ThrowIfTrue(
            subtotal < 0 || discount < 0 || shipping < 0 || total < 0,
            ErrorMessages.Cart.PricePreviewInvalid,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            currency,
            ErrorMessages.ProductListing.CurrencyRequired,
            ErrorMessages.Exception.CommerceTitle);

        PreviewSubtotalAmount = subtotal;
        PreviewDiscountAmount = discount;
        PreviewShippingAmount = shipping;
        PreviewTotalAmount = total;
        Currency = currency.Trim().ToUpperInvariant();
    }

    public void MarkCheckedOut()
    {
        Status = CartStatus.CheckedOut;
    }

    public void Clear()
    {
        Status = CartStatus.Cleared;
    }

    private static void Validate(string userId, Guid productListingId, int quantity)
    {
        BusinessException.ThrowIfNullOrWhiteSpace(
            userId,
            ErrorMessages.PurchaseOrder.UserRequired,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            productListingId == Guid.Empty,
            ErrorMessages.PurchaseOrder.ProductListingRequired,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            quantity <= 0,
            ErrorMessages.PurchaseOrder.QuantityMustBePositive,
            ErrorMessages.Exception.CommerceTitle);
    }
}
