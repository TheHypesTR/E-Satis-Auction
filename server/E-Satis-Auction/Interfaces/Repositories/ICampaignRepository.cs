using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Models.Commerce;

namespace E_Satis_Auction.Interfaces.Repositories;

public interface ICampaignRepository : IGenericRepository<Campaign>
{
    Task<Campaign?> GetWithProductsByIdAsync(Guid id, bool enableTracking = false, CancellationToken cancellationToken = default);
    Task<List<Campaign>> GetActiveLineCampaignsAsync(DateTimeOffset now, CancellationToken cancellationToken = default);
    Task<List<Campaign>> GetActiveFreeShippingCampaignsAsync(DateTimeOffset now, CancellationToken cancellationToken = default);
    Task<Campaign?> GetActiveCouponByCodeAsync(string couponCode, DateTimeOffset now, CancellationToken cancellationToken = default);
}
