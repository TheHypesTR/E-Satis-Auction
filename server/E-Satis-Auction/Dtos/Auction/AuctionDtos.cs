using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Auction;

public sealed record AuctionSummaryDto(
    Guid Id,
    Guid ProductListingId,
    Guid ProductId,
    string ProductName,
    string Sku,
    decimal CurrentPrice,
    decimal MinimumNextBid,
    decimal MinimumBidIncrement,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    AuctionStatus Status,
    Guid? CurrentWinningBidId,
    string? LeadingUserId,
    int Quantity,
    string Currency,
    uint Version);

public sealed record AuctionDetailDto(
    Guid Id,
    Guid ProductListingId,
    Guid ProductId,
    string ProductName,
    string Sku,
    string? SellerUserId,
    decimal StartingPrice,
    decimal CurrentPrice,
    decimal MinimumNextBid,
    decimal MinimumBidIncrement,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    DateTimeOffset OriginalEndsAt,
    AuctionStatus Status,
    Guid? CurrentWinningBidId,
    string? WinningUserId,
    decimal? WinningBidAmount,
    Guid? PurchaseOrderId,
    Guid? PaymentAttemptId,
    int Quantity,
    string Currency,
    decimal WaitingFeeAmount,
    decimal ServiceFeeAmount,
    decimal SellerPayoutAmount,
    decimal PlatformRevenueAmount,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    uint Version);

public sealed record AuctionBidDto(
    Guid Id,
    Guid AuctionId,
    string BidderUserId,
    decimal Amount,
    AuctionBidStatus Status,
    DateTime CreatedAt,
    uint Version);

public sealed record AuctionWinnerDto(
    Guid AuctionId,
    Guid? WinningBidId,
    string? WinningUserId,
    decimal? WinningBidAmount,
    Guid? PurchaseOrderId,
    Guid? PaymentAttemptId,
    AuctionStatus Status);

public sealed record AuctionSnapshotDto(
    Guid AuctionId,
    decimal CurrentPrice,
    Guid? CurrentWinningBidId,
    string? LeadingUserId,
    DateTimeOffset EndsAt,
    AuctionStatus Status,
    decimal MinimumNextBid,
    DateTimeOffset ServerTimeUtc);

public sealed record AuctionPaymentInitiationDto(
    AuctionSnapshotDto Snapshot,
    Commerce.PaymentInitiationDto Payment);
