using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Validators;
using FluentValidation;

namespace e_Sat_Auction.Features.Facility.GetAllFacilities;

public class GetAllFacilitiesQueryValidator : PaginatedQueryValidator<GetAllFacilitiesQuery>
{
    public GetAllFacilitiesQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(128).WithMessage(ErrorMessages.Validation.SearchTermLength);
    }
}