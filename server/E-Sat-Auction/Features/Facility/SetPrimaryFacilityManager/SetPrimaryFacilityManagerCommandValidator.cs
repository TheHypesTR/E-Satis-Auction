using e_Sat_Auction.Common.Constants;
using FluentValidation;

namespace e_Sat_Auction.Features.Facility.SetPrimaryFacilityManager;

public class SetPrimaryFacilityManagerCommandValidator : AbstractValidator<SetPrimaryFacilityManagerCommand>
{
    public SetPrimaryFacilityManagerCommandValidator()
    {
        RuleFor(x => x.FacilityId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
    }
}