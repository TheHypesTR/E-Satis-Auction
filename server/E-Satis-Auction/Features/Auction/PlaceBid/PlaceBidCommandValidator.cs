using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Auction.PlaceBid;

public sealed class PlaceBidCommandValidator : AbstractValidator<PlaceBidCommand>
{
    public PlaceBidCommandValidator()
    {
        RuleFor(command => command.AuctionId)
            .NotEmpty().WithMessage(ErrorMessages.Auction.EntityName);

        RuleFor(command => command.Payload.Amount)
            .GreaterThan(0).WithMessage(ErrorMessages.Auction.BidAmountInvalid);

        RuleFor(command => command.Payload.IdempotencyKey)
            .NotEmpty().WithMessage(ErrorMessages.Payment.IdempotencyKeyRequired)
            .MaximumLength(128).WithMessage(ErrorMessages.Payment.IdempotencyKeyMaxLength);
    }
}
