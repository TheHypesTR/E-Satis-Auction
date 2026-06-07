using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;

namespace E_Satis_Auction.Features.Campaign.UpdateCampaign;

public sealed record UpdateCampaignCommand : ICommand<CampaignDto>
{
    public Guid CampaignId { get; }
    public UpdateCampaignRequest Payload { get; }

    public UpdateCampaignCommand(Guid campaignId, UpdateCampaignRequest payload)
    {
        CampaignId = campaignId;
        Payload = payload;
    }
}
