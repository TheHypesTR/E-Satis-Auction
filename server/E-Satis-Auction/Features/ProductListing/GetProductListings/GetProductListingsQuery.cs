using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;

namespace E_Satis_Auction.Features.ProductListing.GetProductListings;

public sealed record GetProductListingsQuery(
    string? SearchTerm = null,
    Guid? ProductId = null,
    Guid? CategoryId = null,
    Guid? SourceFacilityId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PaginatedList<ProductListingSummaryDto>>, IPaginatedQuery;
