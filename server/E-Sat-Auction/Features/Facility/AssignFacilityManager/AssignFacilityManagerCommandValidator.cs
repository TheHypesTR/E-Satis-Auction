using e_Sat_Auction.Common.Constants;
using FluentValidation;

namespace e_Sat_Auction.Features.Facility.AssignFacilityManager;

public class AssignFacilityManagerCommandValidator : AbstractValidator<AssignFacilityManagerCommand>
{
    public AssignFacilityManagerCommandValidator()
    {
        RuleFor(x => x.FacilityId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(ErrorMessages.Validation.FirstNameRequired)
            .Length(2, 64).WithMessage(ErrorMessages.Validation.FirstNameLength);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(ErrorMessages.Validation.LastNameRequired)
            .Length(2, 64).WithMessage(ErrorMessages.Validation.LastNameLength);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ErrorMessages.Validation.EmailRequired)
            .EmailAddress().WithMessage(ErrorMessages.Validation.InvalidEmail);
    }
}