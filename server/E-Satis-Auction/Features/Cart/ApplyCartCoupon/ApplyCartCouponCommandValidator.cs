using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Cart.ApplyCartCoupon;

public sealed class ApplyCartCouponCommandValidator : AbstractValidator<ApplyCartCouponCommand>
{
    public ApplyCartCouponCommandValidator()
    {
        RuleFor(command => command.Payload.CouponCode)
            .NotEmpty().WithMessage(ErrorMessages.Campaign.CouponCodeRequired)
            .MaximumLength(64).WithMessage(ErrorMessages.Campaign.CouponCodeMaxLength);
    }
}
