using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Auction;

namespace E_Satis_Auction.Features.AdminAuction.CancelAuction;

public sealed record CancelAuctionCommand(Guid AuctionId) : IAuditableCommand<AuctionDetailDto>;
