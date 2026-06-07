using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Payment.ConfirmPayment;

public sealed class ConfirmPaymentCommandValidator : AbstractValidator<ConfirmPaymentCommand>
{
    public ConfirmPaymentCommandValidator()
    {
        RuleFor(command => command.PaymentAttemptId)
            .NotEmpty().WithMessage(ErrorMessages.Payment.EntityName);

        RuleFor(command => command.Payload.IdempotencyKey)
            .NotEmpty().WithMessage(ErrorMessages.Payment.IdempotencyKeyRequired)
            .MaximumLength(128).WithMessage(ErrorMessages.Payment.IdempotencyKeyMaxLength);
    }
}
