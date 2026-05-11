using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Product;

namespace E_Satis_Auction.Features.Product.GetAllProducts;

public sealed record GetAllProductsQuery(string? SearchTerm = null, bool? IsActive = null, int PageNumber = 1, int PageSize = 10)
    : IQuery<PaginatedList<ProductSummaryDto>>, IPaginatedQuery;