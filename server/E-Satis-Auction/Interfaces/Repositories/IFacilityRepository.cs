using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Dtos.Facility;
using E_Satis_Auction.Models.Facilities;

namespace E_Satis_Auction.Interfaces.Repositories;

public interface IFacilityRepository : IGenericRepository<Facility>
{
    Task<Facility?> GetWithDependentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Facility?> GetWithDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Facility?> GetWithManagersByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, FacilityStockLookupDto>> GetFacilityStockInfoByIdsAsync(IEnumerable<Guid> facilityIds, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, string>> GetFacilityNamesByIdsAsync(IEnumerable<Guid> facilityIds, CancellationToken cancellationToken = default);
}