using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Models.Commerce;

public sealed class Campaign : BaseEntity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DiscountType DiscountType { get; private set; }
    public decimal DiscountValue { get; private set; }
    public string? Currency { get; private set; }
    public CampaignStatus Status { get; private set; }
    public DateTimeOffset StartsAt { get; private set; }
    public DateTimeOffset EndsAt { get; private set; }
    public uint Version { get; private set; }

    private readonly List<CampaignProduct> _products = [];
    public IReadOnlyCollection<CampaignProduct> Products => _products;

    private Campaign()
    {
        Name = string.Empty;
        Status = CampaignStatus.Draft;
    }

    public static Campaign Create(
        string name,
        string? description,
        DiscountType discountType,
        decimal discountValue,
        string? currency,
        DateTimeOffset startsAt,
        DateTimeOffset endsAt)
    {
        Validate(name, discountType, discountValue, currency, startsAt, endsAt);

        return new Campaign
        {
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            DiscountType = discountType,
            DiscountValue = discountValue,
            Currency = string.IsNullOrWhiteSpace(currency) ? null : currency.Trim().ToUpperInvariant(),
            Status = CampaignStatus.Draft,
            StartsAt = startsAt,
            EndsAt = endsAt
        };
    }

    public void AddProduct(Guid productId)
    {
        BusinessException.ThrowIfTrue(
            productId == Guid.Empty,
            ErrorMessages.Campaign.ProductRequired,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            _products.Any(product => product.ProductId == productId),
            ErrorMessages.Campaign.ProductAlreadyAssigned,
            ErrorMessages.Exception.CommerceTitle);

        _products.Add(CampaignProduct.Create(Id, productId));
    }

    public void RemoveProduct(Guid productId)
    {
        CampaignProduct? campaignProduct = _products.FirstOrDefault(product => product.ProductId == productId);
        BusinessException.ThrowIfNull(
            campaignProduct,
            ErrorMessages.Campaign.ProductNotAssigned,
            ErrorMessages.Exception.CommerceTitle);

        _products.Remove(campaignProduct!);
    }

    public void Activate()
    {
        BusinessException.ThrowIfTrue(
            Status is CampaignStatus.Active,
            ErrorMessages.Campaign.AlreadyActive,
            ErrorMessages.Exception.CommerceTitle);

        Status = CampaignStatus.Active;
    }

    public void Suspend()
    {
        BusinessException.ThrowIfTrue(
            Status is CampaignStatus.Suspended,
            ErrorMessages.Campaign.AlreadySuspended,
            ErrorMessages.Exception.CommerceTitle);

        Status = CampaignStatus.Suspended;
    }

    public decimal ApplyDiscount(decimal price, string listingCurrency)
    {
        BusinessException.ThrowIfTrue(
            price <= 0,
            ErrorMessages.ProductListing.PriceMustBePositive,
            ErrorMessages.Exception.CommerceTitle);

        return DiscountType is DiscountType.Percentage
            ? decimal.Round(price * (1 - DiscountValue / 100), 2)
            : ApplyFixedDiscount(price, listingCurrency);
    }

    public bool IsApplicableTo(Guid productId, DateTimeOffset now)
    {
        return Status is CampaignStatus.Active &&
               StartsAt <= now &&
               EndsAt >= now &&
               _products.Any(product => product.ProductId == productId);
    }

    private decimal ApplyFixedDiscount(decimal price, string listingCurrency)
    {
        BusinessException.ThrowIfFalse(
            string.Equals(Currency, listingCurrency, StringComparison.OrdinalIgnoreCase),
            ErrorMessages.Campaign.CurrencyMismatch,
            ErrorMessages.Exception.CommerceTitle);

        decimal discountedPrice = price - DiscountValue;
        return discountedPrice < 0 ? 0 : discountedPrice;
    }

    private static void Validate(
        string name,
        DiscountType discountType,
        decimal discountValue,
        string? currency,
        DateTimeOffset startsAt,
        DateTimeOffset endsAt)
    {
        BusinessException.ThrowIfNullOrWhiteSpace(
            name,
            ErrorMessages.Campaign.NameRequired,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            startsAt >= endsAt,
            ErrorMessages.Campaign.InvalidDateRange,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            discountValue <= 0,
            ErrorMessages.Campaign.DiscountValueMustBePositive,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            discountType is DiscountType.Percentage && discountValue > 100,
            ErrorMessages.Campaign.PercentageDiscountInvalid,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            discountType is DiscountType.FixedAmount && string.IsNullOrWhiteSpace(currency),
            ErrorMessages.Campaign.CurrencyRequiredForFixedDiscount,
            ErrorMessages.Exception.CommerceTitle);
    }
}
