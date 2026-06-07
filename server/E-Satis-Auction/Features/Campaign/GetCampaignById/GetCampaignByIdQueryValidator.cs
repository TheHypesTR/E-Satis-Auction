using E_Satis_Auction.Common.Constants;
using FluentValidation;

namespace E_Satis_Auction.Features.Campaign.GetCampaignById;

public sealed class GetCampaignByIdQueryValidator : AbstractValidator<GetCampaignByIdQuery>
{
    public GetCampaignByIdQueryValidator()
    {
        RuleFor(query => query.CampaignId).NotEmpty().WithMessage(ErrorMessages.Campaign.EntityName);
    }
}
