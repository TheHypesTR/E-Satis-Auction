using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Campaign.DeactivateCampaign;

public sealed class DeactivateCampaignCommandValidator : AbstractValidator<DeactivateCampaignCommand>
{
    public DeactivateCampaignCommandValidator()
    {
        RuleFor(command => command.CampaignId).NotEmpty().WithMessage(ErrorMessages.Campaign.EntityName);
    }
}
