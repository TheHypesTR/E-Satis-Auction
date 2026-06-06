using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Campaign.GetCampaignById;

public sealed class GetCampaignByIdQueryHandler : IQueryHandler<GetCampaignByIdQuery, CampaignDto>
{
    private readonly ICampaignRepository _campaignRepository;

    public GetCampaignByIdQueryHandler(ICampaignRepository campaignRepository)
    {
        _campaignRepository = campaignRepository;
    }

    public async Task<CampaignDto> Handle(GetCampaignByIdQuery query, CancellationToken cancellationToken)
    {
        Models.Commerce.Campaign? campaign = await _campaignRepository.GetByIdAsync(query.CampaignId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(campaign, ErrorMessages.Campaign.EntityName, query.CampaignId);
        return CommerceDtoMapper.ToCampaignDto(campaign!);
    }
}
