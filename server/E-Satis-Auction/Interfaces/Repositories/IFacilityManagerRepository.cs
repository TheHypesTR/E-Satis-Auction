using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Models.Facilities;

namespace E_Satis_Auction.Interfaces.Repositories;

public interface IFacilityManagerRepository : IGenericRepository<FacilityManager>
{
    Task<List<Guid>> GetFacilityIdsByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<FacilityManager?> FindManagerAsync(Guid facilityId, string userId, CancellationToken cancellationToken = default);
    Task<FacilityManager?> GetPrimaryManagerAsync(Guid facilityId, CancellationToken cancellationToken = default);
    Task<FacilityManager?> GetOldestManagerAsync(Guid facilityId, string excludedUserId, CancellationToken cancellationToken = default);
    Task<bool> IsManagerExistsAsync(Guid facilityId, string userId, CancellationToken cancellationToken = default);
}