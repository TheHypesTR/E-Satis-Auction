using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Common.Models;
using e_Sat_Auction.Dtos.Category;

namespace e_Sat_Auction.Features.Category.GetAllCategories;

public sealed record GetAllCategoriesQuery(string? SearchTerm = null, bool? IsActive = null, int PageNumber = 1, int PageSize = 10)
    : IQuery<PaginatedList<CategoryDto>>, IPaginatedQuery;