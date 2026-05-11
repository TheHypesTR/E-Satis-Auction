using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Address;
using E_Satis_Auction.Dtos.Facility;
using E_Satis_Auction.Dtos.Manager;
using E_Satis_Auction.Dtos.User;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Features.Facility.GetFacilityById;

using Models.Facilities;

public class GetFacilityByIdQueryHandler : IQueryHandler<GetFacilityByIdQuery, FacilityDetailDto>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<AppUser> _userManager;

    public GetFacilityByIdQueryHandler(
        IFacilityRepository facilityRepository,
        ICurrentUserService currentUserService,
        UserManager<AppUser> userManager)
    {
        _facilityRepository = facilityRepository;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<FacilityDetailDto> Handle(GetFacilityByIdQuery query, CancellationToken cancellationToken)
    {
        Facility? facility = await _facilityRepository.GetWithDetailsByIdAsync(query.Id, cancellationToken);
        NotFoundException.ThrowIfNull(facility, ErrorMessages.Facility.EntityName, query.Id);

        bool hasAccess = await _currentUserService.HasFacilityAccess(facility!.Id, cancellationToken);
        ForbiddenAccessException.ThrowIfFalse(
            hasAccess,
            ErrorMessages.Facility.UnauthorizedFacilityAccess,
            ErrorMessages.Exception.UnauthorizedAccess);

        Dictionary<string, UserLookupDto> userLookup = await GetUsersForManagersAsync(facility.Managers, cancellationToken);

        return MapToDetailDto(facility, userLookup);
    }
    
    private async Task<Dictionary<string, UserLookupDto>> GetUsersForManagersAsync(IEnumerable<FacilityManager> managers, CancellationToken cancellationToken)
    {
        List<string> managerUserIds = managers.Select(m => m.UserId).Distinct().ToList();
        if (managerUserIds.Count is 0)
        {
            return [];
        }

        return await _userManager.Users
            .AsNoTracking()
            .Where(u => managerUserIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FirstName, u.LastName, u.Email })
            .ToDictionaryAsync(
                u => u.Id, 
                u => new UserLookupDto(u.FirstName, u.LastName, u.Email ?? string.Empty), 
                cancellationToken);
    }

    private static FacilityDetailDto MapToDetailDto(Facility facility, Dictionary<string, UserLookupDto> userLookup)
    {
        return new FacilityDetailDto(
            facility.Id,
            facility.Name,
            facility.Description,
            facility.Status.ToString(),
            new AddressDto(
                facility.Address.Title,
                facility.Address.City,
                facility.Address.District,
                facility.Address.OpenAddress,
                facility.Address.Latitude,
                facility.Address.Longitude,
                facility.Address.IsTemporary),
            facility.Managers.Select(m => 
            {
                userLookup.TryGetValue(m.UserId, out UserLookupDto? user);

                return new ManagerDto(
                    m.UserId,
                    user?.FirstName ?? string.Empty,
                    user?.LastName ?? string.Empty,
                    user?.Email ?? string.Empty,
                    m.IsPrimary);
            }).ToList()
        );
    }
}