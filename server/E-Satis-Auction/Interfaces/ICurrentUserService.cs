namespace E_Satis_Auction.Interfaces;

public interface ICurrentUserService
{
    Task<IReadOnlyCollection<Guid>> GetAccessibleFacilityIdsAsync(CancellationToken cancellationToken = default);
    Task<bool> HasFacilityAccess(Guid facilityId, CancellationToken cancellationToken = default);
    string UserId { get; }
    bool IsGeneralAdmin { get; }
    bool IsInRole(string roleName);
}