using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Enums;
using FluentValidation;

namespace E_Satis_Auction.Features.Campaign.UpdateCampaign;

public sealed class UpdateCampaignCommandValidator : AbstractValidator<UpdateCampaignCommand>
{
    public UpdateCampaignCommandValidator()
    {
        RuleFor(command => command.CampaignId).NotEmpty().WithMessage(ErrorMessages.Campaign.EntityName);
        RuleFor(command => command.Payload.Name).NotEmpty().WithMessage(ErrorMessages.Campaign.NameRequired).MaximumLength(128);
        RuleFor(command => command.Payload.Description).MaximumLength(512);
        RuleFor(command => command.Payload.CouponCode).MaximumLength(64).WithMessage(ErrorMessages.Campaign.CouponCodeMaxLength);
        RuleFor(command => command.Payload.Scope).IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
        RuleFor(command => command.Payload.DiscountType).IsInEnum().WithMessage(ErrorMessages.Validation.InvalidIdentifier);
        RuleFor(command => command.Payload.DiscountValue)
            .GreaterThan(0).WithMessage(ErrorMessages.Campaign.DiscountValueMustBePositive)
            .When(command => command.Payload.Scope is not CampaignScope.FreeShipping);
        RuleFor(command => command.Payload.DiscountValue)
            .Equal(0).WithMessage(ErrorMessages.Campaign.FreeShippingDiscountMustBeZero)
            .When(command => command.Payload.Scope is CampaignScope.FreeShipping);
        RuleFor(command => command.Payload.Currency).Length(3).When(command => !string.IsNullOrWhiteSpace(command.Payload.Currency));
        RuleFor(command => command.Payload)
            .Must(payload => !payload.StartsAt.HasValue || !payload.EndsAt.HasValue || payload.StartsAt.Value < payload.EndsAt.Value)
            .WithMessage(ErrorMessages.Campaign.InvalidDateRange)
            .WithName("DateRange");
    }
}
