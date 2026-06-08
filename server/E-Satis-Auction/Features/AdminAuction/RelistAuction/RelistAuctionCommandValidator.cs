using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.AdminAuction.RelistAuction;

public sealed class RelistAuctionCommandValidator : AbstractValidator<RelistAuctionCommand>
{
    public RelistAuctionCommandValidator()
    {
        RuleFor(command => command.AuctionId)
            .NotEmpty().WithMessage(ErrorMessages.Auction.EntityName);

        RuleFor(command => command.Payload)
            .Must(payload => payload.StartsAt < payload.EndsAt)
            .WithMessage(ErrorMessages.Auction.InvalidDateRange)
            .WithName("StartsAt");
    }
}
