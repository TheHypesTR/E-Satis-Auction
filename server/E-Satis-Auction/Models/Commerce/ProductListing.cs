using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Models.Commerce;

public sealed class ProductListing : BaseEntity
{
    public Guid ProductId { get; private set; }
    public Guid SourceFacilityId { get; private set; }
    public decimal SalePrice { get; private set; }
    public string Currency { get; private set; }
    public ProductListingStatus Status { get; private set; }
    public DateTimeOffset? ActiveFrom { get; private set; }
    public DateTimeOffset? ActiveUntil { get; private set; }
    public uint Version { get; private set; }

    private ProductListing()
    {
        Currency = string.Empty;
        Status = ProductListingStatus.Draft;
    }

    public static ProductListing Create(
        Guid productId,
        Guid sourceFacilityId,
        decimal salePrice,
        string currency,
        DateTimeOffset? activeFrom = null,
        DateTimeOffset? activeUntil = null)
    {
        ValidateProductAndFacility(productId, sourceFacilityId);
        ValidatePriceAndCurrency(salePrice, currency);
        ValidateDateRange(activeFrom, activeUntil);

        return new ProductListing
        {
            ProductId = productId,
            SourceFacilityId = sourceFacilityId,
            SalePrice = salePrice,
            Currency = currency.Trim().ToUpperInvariant(),
            Status = ProductListingStatus.Draft,
            ActiveFrom = activeFrom,
            ActiveUntil = activeUntil
        };
    }

    public void UpdatePrice(decimal salePrice, string currency)
    {
        ValidatePriceAndCurrency(salePrice, currency);

        SalePrice = salePrice;
        Currency = currency.Trim().ToUpperInvariant();
    }

    public void UpdateAvailability(DateTimeOffset? activeFrom, DateTimeOffset? activeUntil)
    {
        ValidateDateRange(activeFrom, activeUntil);

        ActiveFrom = activeFrom;
        ActiveUntil = activeUntil;
    }

    public void Activate()
    {
        BusinessException.ThrowIfTrue(
            Status is ProductListingStatus.Active,
            ErrorMessages.ProductListing.AlreadyActive,
            ErrorMessages.Exception.CommerceTitle);

        Status = ProductListingStatus.Active;
    }

    public void Suspend()
    {
        BusinessException.ThrowIfTrue(
            Status is ProductListingStatus.Suspended,
            ErrorMessages.ProductListing.AlreadySuspended,
            ErrorMessages.Exception.CommerceTitle);

        Status = ProductListingStatus.Suspended;
    }

    public void Deactivate()
    {
        BusinessException.ThrowIfTrue(
            Status is ProductListingStatus.Suspended,
            ErrorMessages.ProductListing.AlreadyInactive,
            ErrorMessages.Exception.CommerceTitle);

        Status = ProductListingStatus.Suspended;
    }

    public void Archive()
    {
        BusinessException.ThrowIfTrue(
            Status is ProductListingStatus.Archived,
            ErrorMessages.ProductListing.AlreadyArchived,
            ErrorMessages.Exception.CommerceTitle);

        Status = ProductListingStatus.Archived;
    }

    public bool IsSellableAt(DateTimeOffset now)
    {
        return Status is ProductListingStatus.Active &&
               (!ActiveFrom.HasValue || ActiveFrom.Value <= now) &&
               (!ActiveUntil.HasValue || ActiveUntil.Value >= now);
    }

    private static void ValidateProductAndFacility(Guid productId, Guid sourceFacilityId)
    {
        BusinessException.ThrowIfTrue(
            productId == Guid.Empty,
            ErrorMessages.ProductListing.ProductRequired,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            sourceFacilityId == Guid.Empty,
            ErrorMessages.ProductListing.SourceFacilityRequired,
            ErrorMessages.Exception.CommerceTitle);
    }

    private static void ValidatePriceAndCurrency(decimal salePrice, string currency)
    {
        BusinessException.ThrowIfTrue(
            salePrice <= 0,
            ErrorMessages.ProductListing.PriceMustBePositive,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            currency,
            ErrorMessages.ProductListing.CurrencyRequired,
            ErrorMessages.Exception.CommerceTitle);
    }

    private static void ValidateDateRange(DateTimeOffset? activeFrom, DateTimeOffset? activeUntil)
    {
        BusinessException.ThrowIfTrue(
            activeFrom.HasValue && activeUntil.HasValue && activeFrom.Value > activeUntil.Value,
            ErrorMessages.ProductListing.InvalidActiveDateRange,
            ErrorMessages.Exception.CommerceTitle);
    }
}
