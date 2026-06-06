using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Campaign.UpdateCampaign;

public sealed class UpdateCampaignCommandHandler : ICommandHandler<UpdateCampaignCommand, CampaignDto>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCampaignCommandHandler(ICampaignRepository campaignRepository, IUnitOfWork unitOfWork)
    {
        _campaignRepository = campaignRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CampaignDto> Handle(UpdateCampaignCommand command, CancellationToken cancellationToken)
    {
        Models.Commerce.Campaign? campaign = await _campaignRepository.GetByIdAsync(command.CampaignId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(campaign, ErrorMessages.Campaign.EntityName, command.CampaignId);

        campaign!.Update(
            command.Payload.Name,
            command.Payload.Description,
            command.Payload.CouponCode,
            command.Payload.Scope,
            command.Payload.DiscountType,
            command.Payload.DiscountValue,
            command.Payload.MinimumOrderAmount,
            command.Payload.ProductListingId,
            command.Payload.CategoryId,
            command.Payload.Currency,
            NormalizeStartsAt(command.Payload.StartsAt),
            NormalizeEndsAt(command.Payload.EndsAt));

        _campaignRepository.Update(campaign);
        await _unitOfWork.CompleteAsync(cancellationToken);
        return CommerceDtoMapper.ToCampaignDto(campaign);
    }

    private static DateTimeOffset NormalizeStartsAt(DateTimeOffset? startsAt)
    {
        return startsAt ?? DateTimeOffset.MinValue;
    }

    private static DateTimeOffset NormalizeEndsAt(DateTimeOffset? endsAt)
    {
        return endsAt ?? DateTimeOffset.MaxValue;
    }
}
