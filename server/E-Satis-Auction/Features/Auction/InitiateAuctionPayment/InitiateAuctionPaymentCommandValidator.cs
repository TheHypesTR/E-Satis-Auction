using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Auction.InitiateAuctionPayment;

public sealed class InitiateAuctionPaymentCommandValidator : AbstractValidator<InitiateAuctionPaymentCommand>
{
    public InitiateAuctionPaymentCommandValidator()
    {
        RuleFor(command => command.AuctionId)
            .NotEmpty().WithMessage(ErrorMessages.Auction.EntityName);

        RuleFor(command => command.Payload.IdempotencyKey)
            .NotEmpty().WithMessage(ErrorMessages.Payment.IdempotencyKeyRequired)
            .MaximumLength(128).WithMessage(ErrorMessages.Payment.IdempotencyKeyMaxLength);
    }
}
