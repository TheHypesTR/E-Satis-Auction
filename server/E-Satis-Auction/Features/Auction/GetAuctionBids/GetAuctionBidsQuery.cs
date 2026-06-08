using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Auction;

namespace E_Satis_Auction.Features.Auction.GetAuctionBids;

public sealed record GetAuctionBidsQuery(
    Guid AuctionId,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PaginatedList<AuctionBidDto>>, IPaginatedQuery;
