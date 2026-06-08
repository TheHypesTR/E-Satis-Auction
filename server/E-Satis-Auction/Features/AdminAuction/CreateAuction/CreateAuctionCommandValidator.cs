using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.AdminAuction.CreateAuction;

public sealed class CreateAuctionCommandValidator : AbstractValidator<CreateAuctionCommand>
{
    public CreateAuctionCommandValidator()
    {
        RuleFor(command => command.Payload.ProductListingId)
            .NotEmpty().WithMessage(ErrorMessages.Auction.ProductListingRequired);

        RuleFor(command => command.Payload.StartingPrice)
            .GreaterThanOrEqualTo(0).WithMessage(ErrorMessages.Auction.StartingPriceInvalid);

        RuleFor(command => command.Payload.MinimumBidIncrement)
            .GreaterThan(0).WithMessage(ErrorMessages.Auction.MinimumBidIncrementInvalid);

        RuleFor(command => command.Payload.Quantity)
            .GreaterThan(0).WithMessage(ErrorMessages.Auction.QuantityInvalid);

        RuleFor(command => command.Payload.Currency)
            .NotEmpty().WithMessage(ErrorMessages.Auction.CurrencyRequired)
            .Length(3).WithMessage(ErrorMessages.ProductListing.InvalidCurrency);

        RuleFor(command => command.Payload)
            .Must(payload => payload.StartsAt < payload.EndsAt)
            .WithMessage(ErrorMessages.Auction.InvalidDateRange)
            .WithName("StartsAt");
    }
}
