using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Auction;

namespace E_Satis_Auction.Features.AdminAuction.FinalizeAuction;

public sealed record FinalizeAuctionCommand(Guid AuctionId) : IAuditableCommand<AuctionDetailDto>;
