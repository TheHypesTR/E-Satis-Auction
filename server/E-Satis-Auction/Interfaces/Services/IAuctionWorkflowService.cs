using E_Satis_Auction.Dtos.Auction;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Models.Commerce;

namespace E_Satis_Auction.Interfaces.Services;

public interface IAuctionWorkflowService
{
    Task<AuctionDetailDto> ActivateAuctionAsync(Guid auctionId, CancellationToken cancellationToken = default);
    Task<AuctionDetailDto> CancelAuctionAsync(Guid auctionId, CancellationToken cancellationToken = default);
    Task<AuctionDetailDto> FinalizeAuctionAsync(Guid auctionId, CancellationToken cancellationToken = default);
    Task<PaymentInitiationDto> InitiateWinnerPaymentAsync(Guid auctionId, string userId, string idempotencyKey, CancellationToken cancellationToken = default);
    Task MarkAuctionPaymentSucceededAsync(Auction auction, CancellationToken cancellationToken = default);
    Task MarkAuctionPaymentFailedAsync(Auction auction, CancellationToken cancellationToken = default);
}
