using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Auction;

namespace E_Satis_Auction.Features.Auction.GetAuctionById;

public sealed record GetAuctionByIdQuery(Guid AuctionId) : IQuery<AuctionDetailDto>;
