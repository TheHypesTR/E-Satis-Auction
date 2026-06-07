namespace E_Satis_Auction.Dtos.Commerce.Requests;

public sealed record UpdateCartListingRequest(Guid ProductListingId, int Quantity);
