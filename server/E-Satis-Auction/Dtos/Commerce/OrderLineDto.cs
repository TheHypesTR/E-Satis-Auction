namespace E_Satis_Auction.Dtos.Commerce;

public sealed record OrderLineDto(
    Guid Id,
    Guid ProductId,
    Guid ProductListingId,
    Guid? CampaignId,
    string ProductName,
    string Sku,
    decimal UnitPrice,
    decimal DiscountedUnitPrice,
    int Quantity,
    string Currency,
    IReadOnlyCollection<OrderLineAllocationDto> Allocations);
