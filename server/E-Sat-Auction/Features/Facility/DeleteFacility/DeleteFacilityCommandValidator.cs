using e_Sat_Auction.Common.Constants;
using FluentValidation;

namespace e_Sat_Auction.Features.Facility.DeleteFacility;

public class DeleteFacilityCommandValidator : AbstractValidator<DeleteFacilityCommand>
{
    public DeleteFacilityCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
    }
}