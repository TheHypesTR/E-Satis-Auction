using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Facility.SetPrimaryFacilityManager;

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