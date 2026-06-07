using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.PurchaseOrder.BuyNow;

public sealed class BuyNowCommandValidator : AbstractValidator<BuyNowCommand>
{
    public BuyNowCommandValidator()
    {
        RuleFor(command => command.ProductListingId)
            .NotEmpty().WithMessage(ErrorMessages.Validation.InvalidIdentifier);

        RuleFor(command => command.Quantity)
            .GreaterThan(0).WithMessage(ErrorMessages.PurchaseOrder.QuantityMustBePositive);

        RuleFor(command => command.IdempotencyKey)
            .MaximumLength(128).WithMessage(ErrorMessages.Payment.IdempotencyKeyMaxLength)
            .When(command => !string.IsNullOrWhiteSpace(command.IdempotencyKey));
    }
}
