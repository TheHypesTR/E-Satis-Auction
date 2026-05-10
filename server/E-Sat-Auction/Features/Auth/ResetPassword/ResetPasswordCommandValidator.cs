using e_Sat_Auction.Common.Constants;
using FluentValidation;

namespace e_Sat_Auction.Features.Auth.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.EncryptedPayload)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidResetLink);

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage(ErrorMessages.Validation.PasswordRequired)
            .MinimumLength(8).WithMessage(ErrorMessages.Validation.PasswordMinLength)
            .Matches("[A-Z]").WithMessage(ErrorMessages.Validation.PasswordUpper)
            .Matches("[a-z]").WithMessage(ErrorMessages.Validation.PasswordLower)
            .Matches("[0-9]").WithMessage(ErrorMessages.Validation.PasswordNumber);

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage(ErrorMessages.Validation.ConfirmPassword)
            .Equal(x => x.NewPassword).WithMessage(ErrorMessages.Validation.ConfirmPassword);
    }
}