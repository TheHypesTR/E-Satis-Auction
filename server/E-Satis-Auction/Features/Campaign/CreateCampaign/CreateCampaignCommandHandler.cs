using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Campaign.CreateCampaign;

public sealed class CreateCampaignCommandHandler : ICommandHandler<CreateCampaignCommand, CampaignDto>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCampaignCommandHandler(ICampaignRepository campaignRepository, IUnitOfWork unitOfWork)
    {
        _campaignRepository = campaignRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CampaignDto> Handle(CreateCampaignCommand command, CancellationToken cancellationToken)
    {
        Models.Commerce.Campaign campaign = Models.Commerce.Campaign.Create(
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

        await _campaignRepository.AddAsync(campaign, cancellationToken);
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
