using e_Sat_Auction.Common.Constants;
using FluentValidation;

namespace e_Sat_Auction.Features.Auth.ForgotPassword;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ErrorMessages.Validation.EmailRequired)
            .EmailAddress().WithMessage(ErrorMessages.Validation.InvalidEmail);
    }
}