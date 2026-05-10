using e_Sat_Auction.Interfaces.Repositories;
using e_Sat_Auction.Models.Facilities;
using Microsoft.EntityFrameworkCore;

namespace e_Sat_Auction.Data.Repositories;

public class FacilityManagerRepository : GenericRepository<FacilityManager>, IFacilityManagerRepository
{
    public FacilityManagerRepository(AppDbContext context) : base(context)
    {
    }
    
    public async Task<List<Guid>> GetFacilityIdsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(fm => fm.UserId == userId)
            .Select(fm => fm.FacilityId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<FacilityManager?> FindManagerAsync(Guid facilityId, string userId, CancellationToken cancellationToken = default)
    {
       return await _dbSet.FirstOrDefaultAsync(fm => fm.FacilityId == facilityId && fm.UserId == userId, cancellationToken);
    }

    public async Task<FacilityManager?> GetPrimaryManagerAsync(Guid facilityId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(fm => fm.FacilityId == facilityId && fm.IsPrimary, cancellationToken);
    }

    public async Task<FacilityManager?> GetOldestManagerAsync(Guid facilityId, string excludedUserId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(om => om.FacilityId == facilityId && om.UserId != excludedUserId)
            .OrderBy(om => om.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> IsManagerExistsAsync(Guid facilityId, string userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(om => om.FacilityId == facilityId && om.UserId == userId, cancellationToken);
    }
}