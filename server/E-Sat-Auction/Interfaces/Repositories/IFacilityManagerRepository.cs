using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Models.Facilities;

namespace e_Sat_Auction.Interfaces.Repositories;

public interface IFacilityManagerRepository : IGenericRepository<FacilityManager>
{
    Task<List<Guid>> GetFacilityIdsByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<FacilityManager?> FindManagerAsync(Guid facilityId, string userId, CancellationToken cancellationToken = default);
    Task<FacilityManager?> GetPrimaryManagerAsync(Guid facilityId, CancellationToken cancellationToken = default);
    Task<FacilityManager?> GetOldestManagerAsync(Guid facilityId, string excludedUserId, CancellationToken cancellationToken = default);
    Task<bool> IsManagerExistsAsync(Guid facilityId, string userId, CancellationToken cancellationToken = default);
}