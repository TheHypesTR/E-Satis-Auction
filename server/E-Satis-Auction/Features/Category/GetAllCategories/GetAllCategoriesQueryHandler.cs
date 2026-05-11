using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Category;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Category.GetAllCategories;

using Models.Categories;

public class GetAllCategoriesQueryHandler: IQueryHandler<GetAllCategoriesQuery, PaginatedList<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<PaginatedList<CategoryDto>> Handle(GetAllCategoriesQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Category> categoryQuery = _categoryRepository.GetAllAsQueryable();
        categoryQuery = ApplyFilters(categoryQuery, query);

        IQueryable<CategoryDto> projectedQuery = ProjectToDtoList(categoryQuery);

        return await projectedQuery.ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);
    }

    private static IQueryable<Category> ApplyFilters(IQueryable<Category> query, GetAllCategoriesQuery filters)
    {
        if (filters.IsActive.HasValue)
        {
            query = query.Where(c => c.IsActive == filters.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
        {
            string searchTerm = filters.SearchTerm.ToSemanticCode();
            query = query.Where(c => c.NormalizedName.Contains(searchTerm));
        }
        
        return query;
    }

    private static IQueryable<CategoryDto> ProjectToDtoList(IQueryable<Category> query)
    {
        return query
            .OrderByDescending(category => category.CreatedAt)
            .Select(category => new CategoryDto(
                category.Id,
                category.Name,
                category.Description,
                category.IsActive,
                category.CreatedAt,
                category.UpdatedAt,
                category.Attributes.Select(a => new CategoryAttributeSummaryDto(
                    a.Name,
                    a.DataType,
                    a.Target,
                    a.IsRequired
                )).ToList()
            ));
    }
}