using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Payment.InitiatePayment;

public sealed class InitiatePaymentCommandValidator : AbstractValidator<InitiatePaymentCommand>
{
    public InitiatePaymentCommandValidator()
    {
        RuleFor(command => command.Payload.IdempotencyKey)
            .NotEmpty().WithMessage(ErrorMessages.Payment.IdempotencyKeyRequired)
            .MaximumLength(128).WithMessage(ErrorMessages.Payment.IdempotencyKeyMaxLength);
    }
}
