namespace E_Satis_Auction.Dtos.Auction.Requests;

public sealed record CreateAuctionRequest(
    Guid ProductListingId,
    string? SellerUserId,
    decimal StartingPrice,
    decimal MinimumBidIncrement,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    int Quantity = 1,
    string Currency = "TRY");

public sealed record UpdateAuctionRequest(
    string? SellerUserId,
    decimal StartingPrice,
    decimal MinimumBidIncrement,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    int Quantity = 1,
    string Currency = "TRY");

public sealed record ScheduleAuctionRequest(DateTimeOffset StartsAt, DateTimeOffset EndsAt);

public sealed record RelistAuctionRequest(DateTimeOffset StartsAt, DateTimeOffset EndsAt);

public sealed record PlaceBidRequest(decimal Amount, string IdempotencyKey);

public sealed record InitiateAuctionPaymentRequest(string IdempotencyKey);
