using System.Security.Claims;
using e_Sat_Auction.Common;
using e_Sat_Auction.Interfaces;
using e_Sat_Auction.Interfaces.Repositories;
using e_Sat_Auction.Models.Facilities;

namespace e_Sat_Auction.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IFacilityManagerRepository _facilityManagerRepository;

    public CurrentUserService(
        IFacilityRepository facilityRepository,
        IFacilityManagerRepository facilityManagerRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _facilityRepository = facilityRepository;
        _facilityManagerRepository = facilityManagerRepository;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<IReadOnlyCollection<Guid>> GetAccessibleFacilityIdsAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(UserId))
        {
            return [];
        }

        List<Guid> facilityIds = await _facilityManagerRepository.GetFacilityIdsByUserIdAsync(UserId, cancellationToken);
        return facilityIds;
    }
    
    public async Task<bool> HasFacilityAccess(Guid facilityId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(UserId))
        {
            return false;
        }
        
        if (IsGeneralAdmin)
        {
            return true;
        }

        Facility? facility = await _facilityRepository.GetWithManagersByIdAsync(facilityId, cancellationToken);
        if (facility is null)
        {
            return false;
        }

        return facility.Managers.Any(m => m.UserId == UserId);
    }
    
    public string UserId => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    public bool IsGeneralAdmin => IsInRole(AppRoles.GeneralAdmin);
    
    public bool IsNGOAdmin => IsInRole(AppRoles.NGOAdmin);

    public bool IsInRole(string roleName)
    {
        return _httpContextAccessor.HttpContext?.User.IsInRole(roleName) ?? false;
    }
}