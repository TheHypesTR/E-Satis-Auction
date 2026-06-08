using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Models.Commerce;

public sealed class AuctionBid : BaseEntity
{
    public Guid AuctionId { get; private set; }
    public string BidderUserId { get; private set; }
    public decimal Amount { get; private set; }
    public string IdempotencyKey { get; private set; }
    public AuctionBidStatus Status { get; private set; }
    public uint Version { get; private set; }

    private AuctionBid()
    {
        BidderUserId = string.Empty;
        IdempotencyKey = string.Empty;
        Status = AuctionBidStatus.Accepted;
    }

    public static AuctionBid Create(Guid auctionId, string bidderUserId, decimal amount, string idempotencyKey)
    {
        BusinessException.ThrowIfTrue(auctionId == Guid.Empty, ErrorMessages.Auction.EntityName, ErrorMessages.Exception.CommerceTitle);
        BusinessException.ThrowIfNullOrWhiteSpace(bidderUserId, ErrorMessages.PurchaseOrder.UserRequired, ErrorMessages.Exception.CommerceTitle);
        BusinessException.ThrowIfTrue(amount <= 0, ErrorMessages.Auction.BidAmountInvalid, ErrorMessages.Exception.CommerceTitle);
        BusinessException.ThrowIfNullOrWhiteSpace(idempotencyKey, ErrorMessages.Payment.IdempotencyKeyRequired, ErrorMessages.Exception.CommerceTitle);

        return new AuctionBid
        {
            AuctionId = auctionId,
            BidderUserId = bidderUserId.Trim(),
            Amount = amount,
            IdempotencyKey = idempotencyKey.Trim(),
            Status = AuctionBidStatus.Accepted
        };
    }
}
