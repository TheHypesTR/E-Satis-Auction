using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Dtos.Facility;
using e_Sat_Auction.Models.Facilities;

namespace e_Sat_Auction.Interfaces.Repositories;

public interface IFacilityRepository : IGenericRepository<Facility>
{
    Task<Facility?> GetWithDependentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Facility?> GetWithDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Facility?> GetWithManagersByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, FacilityStockLookupDto>> GetFacilityStockInfoByIdsAsync(IEnumerable<Guid> facilityIds, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, string>> GetFacilityNamesByIdsAsync(IEnumerable<Guid> facilityIds, CancellationToken cancellationToken = default);
    Task<List<Guid>> GetFacilityIdsByOrganizationIdsAsync(IEnumerable<Guid> organizationIds, CancellationToken cancellationToken = default);
}