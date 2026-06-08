using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Auction;
using E_Satis_Auction.Dtos.Auction.Requests;

namespace E_Satis_Auction.Features.AdminAuction.CreateAuction;

public sealed record CreateAuctionCommand(CreateAuctionRequest Payload) : IAuditableCommand<AuctionDetailDto>;
