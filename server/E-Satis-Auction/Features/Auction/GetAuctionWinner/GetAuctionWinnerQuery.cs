using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Auction;

namespace E_Satis_Auction.Features.Auction.GetAuctionWinner;

public sealed record GetAuctionWinnerQuery(Guid AuctionId) : IQuery<AuctionWinnerDto>;
