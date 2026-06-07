using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;

namespace E_Satis_Auction.Features.Campaign.CreateCampaign;

public sealed record CreateCampaignCommand(CreateCampaignRequest Payload) : ICommand<CampaignDto>;
