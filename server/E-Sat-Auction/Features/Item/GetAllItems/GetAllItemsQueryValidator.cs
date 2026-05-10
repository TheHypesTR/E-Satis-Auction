using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Validators;
using FluentValidation;

namespace e_Sat_Auction.Features.Item.GetAllItems;

public sealed class GetAllItemsQueryValidator : PaginatedQueryValidator<GetAllItemsQuery>
{
    public GetAllItemsQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(128).WithMessage(ErrorMessages.Validation.SearchTermLength);
    }
}