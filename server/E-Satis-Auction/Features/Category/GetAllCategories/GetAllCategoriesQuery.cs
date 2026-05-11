using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Category;

namespace E_Satis_Auction.Features.Category.GetAllCategories;

public sealed record GetAllCategoriesQuery(string? SearchTerm = null, bool? IsActive = null, int PageNumber = 1, int PageSize = 10)
    : IQuery<PaginatedList<CategoryDto>>, IPaginatedQuery;