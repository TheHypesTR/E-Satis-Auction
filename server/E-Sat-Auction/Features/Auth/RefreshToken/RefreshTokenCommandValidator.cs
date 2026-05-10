using e_Sat_Auction.Common.Constants;
using FluentValidation;

namespace e_Sat_Auction.Features.Auth.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage(ErrorMessages.Validation.RefreshTokenRequired);
    }
}