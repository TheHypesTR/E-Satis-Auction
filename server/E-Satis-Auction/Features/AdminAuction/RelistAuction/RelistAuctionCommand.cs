using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Auction;
using E_Satis_Auction.Dtos.Auction.Requests;

namespace E_Satis_Auction.Features.AdminAuction.RelistAuction;

public sealed record RelistAuctionCommand(Guid AuctionId, RelistAuctionRequest Payload) : IAuditableCommand<AuctionDetailDto>;
