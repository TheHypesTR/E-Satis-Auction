using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Validators;
using FluentValidation;

namespace e_Sat_Auction.Features.Category.GetAllCategories;

public class GetAllCategoriesQueryValidator : PaginatedQueryValidator<GetAllCategoriesQuery>
{
    public GetAllCategoriesQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(128).WithMessage(ErrorMessages.Validation.SearchTermLength);
    }
}