using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Models.Commerce;

namespace E_Satis_Auction.Interfaces.Repositories;

public interface ICampaignRepository : IGenericRepository<Campaign>
{
    Task<Campaign?> GetWithProductsByIdAsync(Guid id, bool enableTracking = false, CancellationToken cancellationToken = default);
}
