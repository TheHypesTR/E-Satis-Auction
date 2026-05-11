using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Facility.DeleteFacility;

public class DeleteFacilityCommandValidator : AbstractValidator<DeleteFacilityCommand>
{
    public DeleteFacilityCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
    }
}