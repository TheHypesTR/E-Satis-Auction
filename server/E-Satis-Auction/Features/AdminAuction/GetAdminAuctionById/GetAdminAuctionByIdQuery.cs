using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Auction;

namespace E_Satis_Auction.Features.AdminAuction.GetAdminAuctionById;

public sealed record GetAdminAuctionByIdQuery(Guid AuctionId) : IQuery<AuctionDetailDto>;
