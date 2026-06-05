using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Commerce;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Data.Repositories;

public sealed class CampaignRepository : GenericRepository<Campaign>, ICampaignRepository
{
    public CampaignRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Campaign?> GetWithProductsByIdAsync(Guid id, bool enableTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Campaign> query = _dbSet.Include(campaign => campaign.Products);
        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(campaign => campaign.Id == id, cancellationToken);
    }
}
