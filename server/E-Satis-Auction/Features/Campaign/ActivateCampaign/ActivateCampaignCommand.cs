using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;

namespace E_Satis_Auction.Features.Campaign.ActivateCampaign;

public sealed record ActivateCampaignCommand(Guid CampaignId) : ICommand<CampaignDto>;
