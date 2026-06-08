using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Common.Exceptions;

namespace E_Satis_Auction.Models.Commerce;

public sealed class CampaignProduct : BaseEntity
{
    public Guid CampaignId { get; private set; }
    public Guid ProductId { get; private set; }

    private CampaignProduct()
    {
    }

    public static CampaignProduct Create(Guid campaignId, Guid productId)
    {
        BusinessException.ThrowIfTrue(
            campaignId == Guid.Empty,
            ErrorMessages.Campaign.EntityName,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            productId == Guid.Empty,
            ErrorMessages.Campaign.ProductRequired,
            ErrorMessages.Exception.CommerceTitle);

        return new CampaignProduct
        {
            CampaignId = campaignId,
            ProductId = productId
        };
    }
}
