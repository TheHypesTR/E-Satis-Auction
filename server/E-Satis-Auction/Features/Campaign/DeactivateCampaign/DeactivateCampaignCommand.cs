using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;

namespace E_Satis_Auction.Features.Campaign.DeactivateCampaign;

public sealed record DeactivateCampaignCommand(Guid CampaignId) : ICommand<CampaignDto>;
