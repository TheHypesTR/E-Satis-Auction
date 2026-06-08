using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Models.Commerce;

namespace E_Satis_Auction.Dtos.Auction;

public static class AuctionDtoMapper
{
    public static AuctionSummaryDto ToSummaryDto(Models.Commerce.Auction auction, ProductListingProductEnrichmentDto product)
    {
        return new AuctionSummaryDto(
            auction.Id,
            auction.ProductListingId,
            auction.ProductId,
            product.Name,
            product.Sku,
            auction.CurrentPrice,
            auction.MinimumNextBid,
            auction.MinimumBidIncrement,
            auction.StartsAt,
            auction.EndsAt,
            auction.Status,
            auction.CurrentWinningBidId,
            auction.WinningUserId,
            auction.Quantity,
            auction.Currency,
            auction.Version);
    }

    public static AuctionDetailDto ToDetailDto(Models.Commerce.Auction auction, ProductListingProductEnrichmentDto product)
    {
        return new AuctionDetailDto(
            auction.Id,
            auction.ProductListingId,
            auction.ProductId,
            product.Name,
            product.Sku,
            auction.SellerUserId,
            auction.StartingPrice,
            auction.CurrentPrice,
            auction.MinimumNextBid,
            auction.MinimumBidIncrement,
            auction.StartsAt,
            auction.EndsAt,
            auction.OriginalEndsAt,
            auction.Status,
            auction.CurrentWinningBidId,
            auction.WinningUserId,
            auction.WinningBidAmount,
            auction.PurchaseOrderId,
            auction.PaymentAttemptId,
            auction.Quantity,
            auction.Currency,
            auction.WaitingFeeAmount,
            auction.ServiceFeeAmount,
            auction.SellerPayoutAmount,
            auction.PlatformRevenueAmount,
            auction.CreatedAt,
            auction.UpdatedAt,
            auction.Version);
    }

    public static AuctionBidDto ToBidDto(AuctionBid bid)
    {
        return new AuctionBidDto(
            bid.Id,
            bid.AuctionId,
            bid.BidderUserId,
            bid.Amount,
            bid.Status,
            bid.CreatedAt,
            bid.Version);
    }

    public static AuctionWinnerDto ToWinnerDto(Models.Commerce.Auction auction)
    {
        return new AuctionWinnerDto(
            auction.Id,
            auction.CurrentWinningBidId,
            auction.WinningUserId,
            auction.WinningBidAmount,
            auction.PurchaseOrderId,
            auction.PaymentAttemptId,
            auction.Status);
    }

    public static AuctionSnapshotDto ToSnapshotDto(Models.Commerce.Auction auction, DateTimeOffset serverTimeUtc)
    {
        return new AuctionSnapshotDto(
            auction.Id,
            auction.CurrentPrice,
            auction.CurrentWinningBidId,
            auction.WinningUserId,
            auction.EndsAt,
            auction.Status,
            auction.MinimumNextBid,
            serverTimeUtc);
    }
}
