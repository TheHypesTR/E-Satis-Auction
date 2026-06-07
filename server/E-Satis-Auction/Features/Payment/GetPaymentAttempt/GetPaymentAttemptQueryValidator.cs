using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Payment.GetPaymentAttempt;

public sealed class GetPaymentAttemptQueryValidator : AbstractValidator<GetPaymentAttemptQuery>
{
    public GetPaymentAttemptQueryValidator()
    {
        RuleFor(query => query.PaymentAttemptId)
            .NotEmpty().WithMessage(ErrorMessages.Payment.EntityName);
    }
}
