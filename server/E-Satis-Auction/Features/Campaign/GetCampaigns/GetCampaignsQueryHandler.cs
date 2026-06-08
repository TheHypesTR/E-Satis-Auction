using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Campaign.GetCampaigns;

public sealed class GetCampaignsQueryHandler : IQueryHandler<GetCampaignsQuery, PaginatedList<CampaignDto>>
{
    private readonly ICampaignRepository _campaignRepository;

    public GetCampaignsQueryHandler(ICampaignRepository campaignRepository)
    {
        _campaignRepository = campaignRepository;
    }

    public async Task<PaginatedList<CampaignDto>> Handle(GetCampaignsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Models.Commerce.Campaign> campaigns = _campaignRepository.GetAllAsQueryable();
        if (query.Status.HasValue)
        {
            campaigns = campaigns.Where(campaign => campaign.Status == query.Status.Value);
        }

        if (query.Scope.HasValue)
        {
            campaigns = campaigns.Where(campaign => campaign.Scope == query.Scope.Value);
        }

        PaginatedList<Models.Commerce.Campaign> paged = await campaigns
            .OrderByDescending(campaign => campaign.CreatedAt)
            .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

        return new PaginatedList<CampaignDto>(
            paged.Items.Select(CommerceDtoMapper.ToCampaignDto).ToList(),
            paged.TotalCount,
            paged.PageNumber,
            query.PageSize);
    }
}
