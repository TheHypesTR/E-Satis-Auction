using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Campaign.ActivateCampaign;

public sealed class ActivateCampaignCommandValidator : AbstractValidator<ActivateCampaignCommand>
{
    public ActivateCampaignCommandValidator()
    {
        RuleFor(command => command.CampaignId).NotEmpty().WithMessage(ErrorMessages.Campaign.EntityName);
    }
}
