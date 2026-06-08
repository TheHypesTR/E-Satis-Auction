using E_Satis_Auction.Dtos.Auction;
using E_Satis_Auction.Hubs;
using E_Satis_Auction.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace E_Satis_Auction.Services;

public sealed class AuctionRealtimeNotifier : IAuctionRealtimeNotifier
{
    private readonly IHubContext<AuctionHub> _hubContext;

    public AuctionRealtimeNotifier(IHubContext<AuctionHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task BroadcastAuctionSnapshotAsync(AuctionSnapshotDto snapshot, CancellationToken cancellationToken = default)
    {
        await SendAsync(snapshot.AuctionId, "AuctionSnapshot", snapshot, cancellationToken);
    }

    public async Task BroadcastBidAcceptedAsync(AuctionSnapshotDto snapshot, AuctionBidDto bid, CancellationToken cancellationToken = default)
    {
        await SendAsync(snapshot.AuctionId, "BidAccepted", new { Snapshot = snapshot, Bid = bid }, cancellationToken);
    }

    public async Task BroadcastAuctionExtendedAsync(AuctionSnapshotDto snapshot, CancellationToken cancellationToken = default)
    {
        await SendAsync(snapshot.AuctionId, "AuctionExtended", snapshot, cancellationToken);
    }

    public async Task BroadcastAuctionEndedAsync(AuctionSnapshotDto snapshot, CancellationToken cancellationToken = default)
    {
        await SendAsync(snapshot.AuctionId, "AuctionEnded", snapshot, cancellationToken);
    }

    public async Task BroadcastAuctionCancelledAsync(AuctionSnapshotDto snapshot, CancellationToken cancellationToken = default)
    {
        await SendAsync(snapshot.AuctionId, "AuctionCancelled", snapshot, cancellationToken);
    }

    public async Task BroadcastWinnerSelectedAsync(AuctionSnapshotDto snapshot, AuctionWinnerDto winner, CancellationToken cancellationToken = default)
    {
        await SendAsync(snapshot.AuctionId, "WinnerSelected", new { Snapshot = snapshot, Winner = winner }, cancellationToken);
    }

    public async Task BroadcastPaymentWindowStartedAsync(AuctionSnapshotDto snapshot, Guid paymentAttemptId, DateTimeOffset expiresAt, CancellationToken cancellationToken = default)
    {
        await SendAsync(snapshot.AuctionId, "PaymentWindowStarted", new { Snapshot = snapshot, PaymentAttemptId = paymentAttemptId, ExpiresAt = expiresAt }, cancellationToken);
    }

    public async Task BroadcastPaymentExpiredAsync(AuctionSnapshotDto snapshot, CancellationToken cancellationToken = default)
    {
        await SendAsync(snapshot.AuctionId, "PaymentExpired", snapshot, cancellationToken);
    }

    public async Task BroadcastAuctionCompletedAsync(AuctionSnapshotDto snapshot, CancellationToken cancellationToken = default)
    {
        await SendAsync(snapshot.AuctionId, "AuctionCompleted", snapshot, cancellationToken);
    }

    private async Task SendAsync(Guid auctionId, string methodName, object payload, CancellationToken cancellationToken)
    {
        await _hubContext.Clients
            .Group(AuctionHub.GetGroupName(auctionId))
            .SendAsync(methodName, payload, cancellationToken);
    }
}
