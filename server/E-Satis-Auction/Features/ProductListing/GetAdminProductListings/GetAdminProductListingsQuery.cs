using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Features.ProductListing.GetAdminProductListings;

public sealed record GetAdminProductListingsQuery(
    ProductListingStatus? Status = null,
    Guid? ProductId = null,
    Guid? SourceFacilityId = null,
    string? SearchTerm = null,
    Guid? CategoryId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    DateTimeOffset? StartDate = null,
    DateTimeOffset? EndDate = null,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PaginatedList<AdminProductListingSummaryDto>>, IPaginatedQuery;
