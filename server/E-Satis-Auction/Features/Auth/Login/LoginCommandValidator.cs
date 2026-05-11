using System.ComponentModel.DataAnnotations;
using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Helpers;
using FluentValidation;

namespace E_Satis_Auction.Features.Auth.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty().WithMessage(ErrorMessages.Validation.IdentifierRequired)
            .Must(BeAValidEmailOrTCNumber).WithMessage(ErrorMessages.Validation.InvalidUserIdentifier);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ErrorMessages.Validation.PasswordRequired);
    }

    private bool BeAValidEmailOrTCNumber(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return false;
        }

        if (identifier.Contains('@'))
        {
            return new EmailAddressAttribute().IsValid(identifier);
        }

        return TCNoHelper.IsValid(identifier);
    }
}