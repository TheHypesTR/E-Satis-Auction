using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Auction;

namespace E_Satis_Auction.Features.AdminAuction.ActivateAuction;

public sealed record ActivateAuctionCommand(Guid AuctionId) : IAuditableCommand<AuctionDetailDto>;
