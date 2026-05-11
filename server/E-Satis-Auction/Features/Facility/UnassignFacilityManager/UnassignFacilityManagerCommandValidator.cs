using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Facility.UnassignFacilityManager;

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