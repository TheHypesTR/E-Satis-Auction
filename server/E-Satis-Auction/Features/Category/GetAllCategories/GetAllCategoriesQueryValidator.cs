using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Validators;
using FluentValidation;

namespace E_Satis_Auction.Features.Category.GetAllCategories;

public class GetAllCategoriesQueryValidator : PaginatedQueryValidator<GetAllCategoriesQuery>
{
    public GetAllCategoriesQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(128).WithMessage(ErrorMessages.Validation.SearchTermLength);
    }
}