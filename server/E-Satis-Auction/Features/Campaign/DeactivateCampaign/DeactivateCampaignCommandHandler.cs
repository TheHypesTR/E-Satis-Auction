using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Campaign.DeactivateCampaign;

public sealed class DeactivateCampaignCommandHandler : ICommandHandler<DeactivateCampaignCommand, CampaignDto>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateCampaignCommandHandler(ICampaignRepository campaignRepository, IUnitOfWork unitOfWork)
    {
        _campaignRepository = campaignRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CampaignDto> Handle(DeactivateCampaignCommand command, CancellationToken cancellationToken)
    {
        Models.Commerce.Campaign? campaign = await _campaignRepository.GetByIdAsync(command.CampaignId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(campaign, ErrorMessages.Campaign.EntityName, command.CampaignId);
        campaign!.Suspend();
        _campaignRepository.Update(campaign);
        await _unitOfWork.CompleteAsync(cancellationToken);
        return CommerceDtoMapper.ToCampaignDto(campaign);
    }
}
