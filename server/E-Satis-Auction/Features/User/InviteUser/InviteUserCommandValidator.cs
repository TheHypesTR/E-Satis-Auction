using E_Satis_Auction.Common;
using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.User.InviteUser;

public class InviteUserCommandValidator : AbstractValidator<InviteUserCommand>
{
    public InviteUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(ErrorMessages.Validation.FirstNameRequired)
            .Length(2, 64).WithMessage(ErrorMessages.Validation.FirstNameLength);

        RuleFor(x => x.LastName)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(ErrorMessages.Validation.LastNameRequired)
            .Length(2, 64).WithMessage(ErrorMessages.Validation.LastNameLength);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ErrorMessages.Validation.EmailRequired)
            .EmailAddress().WithMessage(ErrorMessages.Validation.InvalidEmail);

        RuleFor(x => x.TargetRole)
            .NotEmpty().WithMessage(ErrorMessages.Validation.TargetRoleRequired)
            .Must(role => AppRoles.AllRoles.Contains(role)).WithMessage(ErrorMessages.Validation.InvalidRole);
    }
}