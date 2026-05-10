using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Validators;
using FluentValidation;

namespace e_Sat_Auction.Features.Product.GetAllProducts;

public class GetAllProductsQueryValidator : PaginatedQueryValidator<GetAllProductsQuery>
{
    public GetAllProductsQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(128).WithMessage(ErrorMessages.Validation.SearchTermLength);
    }
}