namespace E_Satis_Auction.Dtos.Commerce.Requests;

public sealed record UpdateProductListingRequest(
    decimal Price,
    string Currency,
    DateTimeOffset? ActiveFrom = null,
    DateTimeOffset? ActiveUntil = null);
