using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.AdminAuction.ScheduleAuction;

public sealed class ScheduleAuctionCommandValidator : AbstractValidator<ScheduleAuctionCommand>
{
    public ScheduleAuctionCommandValidator()
    {
        RuleFor(command => command.AuctionId)
            .NotEmpty().WithMessage(ErrorMessages.Auction.EntityName);

        RuleFor(command => command.Payload)
            .Must(payload => payload.StartsAt < payload.EndsAt)
            .WithMessage(ErrorMessages.Auction.InvalidDateRange)
            .WithName("StartsAt");
    }
}
