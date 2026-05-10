using e_Sat_Auction.Common.Constants;
using FluentValidation;

namespace e_Sat_Auction.Features.Facility.UnassignFacilityManager;

public class UnassignFacilityManagerCommandValidator : AbstractValidator<UnassignFacilityManagerCommand>
{
    public UnassignFacilityManagerCommandValidator()
    {
        RuleFor(x => x.FacilityId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
    }
}