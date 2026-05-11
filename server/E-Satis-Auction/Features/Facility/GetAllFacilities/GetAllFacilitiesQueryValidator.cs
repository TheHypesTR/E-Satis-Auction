using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Validators;
using FluentValidation;

namespace E_Satis_Auction.Features.Facility.GetAllFacilities;

public class GetAllFacilitiesQueryValidator : PaginatedQueryValidator<GetAllFacilitiesQuery>
{
    public GetAllFacilitiesQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(128).WithMessage(ErrorMessages.Validation.SearchTermLength);
    }
}