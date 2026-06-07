namespace E_Satis_Auction.Dtos.Commerce.Requests;

public sealed record CreateProductListingRequest(
    Guid ProductId,
    Guid SourceFacilityId,
    decimal Price,
    string Currency,
    DateTimeOffset? ActiveFrom = null,
    DateTimeOffset? ActiveUntil = null);
