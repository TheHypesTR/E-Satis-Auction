using E_Satis_Auction.Dtos.Auction;

namespace E_Satis_Auction.Interfaces.Services;

public interface IAuctionRealtimeNotifier
{
    Task BroadcastAuctionSnapshotAsync(AuctionSnapshotDto snapshot, CancellationToken cancellationToken = default);
    Task BroadcastBidAcceptedAsync(AuctionSnapshotDto snapshot, AuctionBidDto bid, CancellationToken cancellationToken = default);
    Task BroadcastAuctionExtendedAsync(AuctionSnapshotDto snapshot, CancellationToken cancellationToken = default);
    Task BroadcastAuctionEndedAsync(AuctionSnapshotDto snapshot, CancellationToken cancellationToken = default);
    Task BroadcastAuctionCancelledAsync(AuctionSnapshotDto snapshot, CancellationToken cancellationToken = default);
    Task BroadcastWinnerSelectedAsync(AuctionSnapshotDto snapshot, AuctionWinnerDto winner, CancellationToken cancellationToken = default);
    Task BroadcastPaymentWindowStartedAsync(AuctionSnapshotDto snapshot, Guid paymentAttemptId, DateTimeOffset expiresAt, CancellationToken cancellationToken = default);
    Task BroadcastPaymentExpiredAsync(AuctionSnapshotDto snapshot, CancellationToken cancellationToken = default);
    Task BroadcastAuctionCompletedAsync(AuctionSnapshotDto snapshot, CancellationToken cancellationToken = default);
}
