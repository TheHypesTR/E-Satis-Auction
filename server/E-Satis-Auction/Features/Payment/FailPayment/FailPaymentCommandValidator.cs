using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Payment.FailPayment;

public sealed class FailPaymentCommandValidator : AbstractValidator<FailPaymentCommand>
{
    public FailPaymentCommandValidator()
    {
        RuleFor(command => command.PaymentAttemptId)
            .NotEmpty().WithMessage(ErrorMessages.Payment.EntityName);

        RuleFor(command => command.Payload.IdempotencyKey)
            .NotEmpty().WithMessage(ErrorMessages.Payment.IdempotencyKeyRequired)
            .MaximumLength(128).WithMessage(ErrorMessages.Payment.IdempotencyKeyMaxLength);

        RuleFor(command => command.Payload.Reason)
            .NotEmpty().WithMessage(ErrorMessages.Payment.FailureReasonRequired)
            .MaximumLength(1024).WithMessage(ErrorMessages.Payment.FailureReasonMaxLength);
    }
}
