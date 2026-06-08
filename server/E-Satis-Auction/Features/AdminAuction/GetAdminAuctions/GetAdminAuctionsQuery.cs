using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Auction;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Features.AdminAuction.GetAdminAuctions;

public sealed record GetAdminAuctionsQuery(
    AuctionStatus? Status = null,
    Guid? ProductListingId = null,
    Guid? ProductId = null,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PaginatedList<AuctionSummaryDto>>, IPaginatedQuery;
