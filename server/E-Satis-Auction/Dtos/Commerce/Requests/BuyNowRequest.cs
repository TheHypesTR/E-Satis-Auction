namespace E_Satis_Auction.Dtos.Commerce.Requests;

public sealed record BuyNowRequest(Guid ProductListingId, int Quantity, Guid? CampaignId = null, string? IdempotencyKey = null);
