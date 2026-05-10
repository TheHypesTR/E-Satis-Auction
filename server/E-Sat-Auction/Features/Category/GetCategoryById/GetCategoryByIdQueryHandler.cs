using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Exceptions;
using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.Category;
using e_Sat_Auction.Interfaces.Repositories;

namespace e_Sat_Auction.Features.Category.GetCategoryById;

using Models.Categories;

public sealed class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, CategoryDetailDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDetailDto> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
    {
        Category? category = await _categoryRepository.GetWithDetailsByIdAsync(query.Id, cancellationToken);
        NotFoundException.ThrowIfNull(category, ErrorMessages.Category.EntityName, query.Id);

        return MapToDetailDto(category!);
    }

    private static CategoryDetailDto MapToDetailDto(Category category)
    {
        return new CategoryDetailDto(
            category.Id,
            category.Name,
            category.Description,
            category.IsActive,
            category.CreatedAt,
            category.UpdatedAt,
            category.Attributes.Select(attribute => new CategoryAttributeDto(
                attribute.Id,
                attribute.Name,
                attribute.Code,
                attribute.DataType,
                attribute.Target,
                attribute.IsRequired,
                attribute.Options.Select(option => new CategoryAttributeOptionDto(
                    option.Id,
                    option.Value)).ToList()
            )).ToList()
        );
    }
}