using E_Satis_Auction.Dtos.Facility;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Facilities;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Data.Repositories;

public class FacilityRepository : GenericRepository<Facility>, IFacilityRepository
{
    public FacilityRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Facility?> GetWithDependentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Address)
            .Include(f => f.Managers)
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<Facility?> GetWithDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Include(o => o.Address)
            .Include(o => o.Managers)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
    
    public async Task<Facility?> GetWithManagersByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Include(o => o.Managers)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Dictionary<Guid, FacilityStockLookupDto>> GetFacilityStockInfoByIdsAsync(IEnumerable<Guid> facilityIds, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(f => f.Address)
            .Where(f => facilityIds.Contains(f.Id))
            .ToDictionaryAsync(
                f => f.Id, 
                f => new FacilityStockLookupDto(
                    f.Name, 
                    new FacilityAddressDto(f.Address.City, f.Address.District, f.Address.OpenAddress)), 
                cancellationToken);
    }

    public async Task<Dictionary<Guid, string>> GetFacilityNamesByIdsAsync(IEnumerable<Guid> facilityIds, CancellationToken cancellationToken = default)
    {
        List<Guid> ids = facilityIds.Distinct().ToList();
        if (ids.Count is 0)
        {
            return [];
        }

        return await _dbSet
            .AsNoTracking()
            .Where(f => ids.Contains(f.Id))
            .Select(f => new { f.Id, f.Name })
            .ToDictionaryAsync(f => f.Id, f => f.Name, cancellationToken);
    }

    public async Task<List<Guid>> GetFacilityIdsByOrganizationIdsAsync(IEnumerable<Guid> organizationIds, CancellationToken cancellationToken = default)
    {
        List<Guid> ids = organizationIds.Distinct().ToList();
        if (ids.Count is 0)
        {
            return [];
        }

        return await _dbSet
            .AsNoTracking()
            .Where(f => ids.Contains(f.OrganizationId))
            .Select(f => f.Id)
            .ToListAsync(cancellationToken);
    }
}