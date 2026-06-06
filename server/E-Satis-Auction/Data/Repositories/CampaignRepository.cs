using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Commerce;
using E_Satis_Auction.Enums;
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

    public async Task<List<Campaign>> GetActiveLineCampaignsAsync(DateTimeOffset now, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(campaign => campaign.Products)
            .Where(campaign =>
                campaign.Status == CampaignStatus.Active &&
                campaign.StartsAt <= now &&
                campaign.EndsAt >= now &&
                (campaign.Scope == CampaignScope.ProductListing || campaign.Scope == CampaignScope.Category))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Campaign>> GetActiveFreeShippingCampaignsAsync(DateTimeOffset now, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(campaign =>
                campaign.Status == CampaignStatus.Active &&
                campaign.StartsAt <= now &&
                campaign.EndsAt >= now &&
                campaign.Scope == CampaignScope.FreeShipping)
            .ToListAsync(cancellationToken);
    }

    public async Task<Campaign?> GetActiveCouponByCodeAsync(string couponCode, DateTimeOffset now, CancellationToken cancellationToken = default)
    {
        string normalizedCode = couponCode.Trim().ToUpperInvariant();
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(campaign =>
                campaign.Status == CampaignStatus.Active &&
                campaign.StartsAt <= now &&
                campaign.EndsAt >= now &&
                campaign.Scope == CampaignScope.CartOrder &&
                campaign.CouponCode == normalizedCode,
                cancellationToken);
    }
}
