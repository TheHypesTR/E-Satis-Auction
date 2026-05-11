using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Validators;
using FluentValidation;

namespace E_Satis_Auction.Features.Dispatch.GetAllDispatches;

public sealed class GetAllDispatchesQueryValidator : PaginatedQueryValidator<GetAllDispatchesQuery>
{
    public GetAllDispatchesQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(128).WithMessage(ErrorMessages.Validation.SearchTermLength);
    }
}