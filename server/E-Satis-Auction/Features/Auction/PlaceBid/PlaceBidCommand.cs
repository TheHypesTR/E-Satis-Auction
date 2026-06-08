using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Auction;
using E_Satis_Auction.Dtos.Auction.Requests;

namespace E_Satis_Auction.Features.Auction.PlaceBid;

public sealed record PlaceBidCommand(Guid AuctionId, PlaceBidRequest Payload) : IAuditableCommand<AuctionBidDto>;
