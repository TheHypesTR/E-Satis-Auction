using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Facility;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Features.Facility.GetMyFacilities;

using Models.Facilities;

public class GetMyFacilitiesQueryHandler : IQueryHandler<GetMyFacilitiesQuery, PaginatedList<FacilityDto>>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyFacilitiesQueryHandler(IFacilityRepository facilityRepository, ICurrentUserService currentUserService)
    {
        _facilityRepository = facilityRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedList<FacilityDto>> Handle(GetMyFacilitiesQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Facility> facilityQuery = _facilityRepository.GetAllAsQueryable()
            .Include(f => f.Address)
            .Where(f => f.Managers.Any(m => m.UserId == _currentUserService.UserId));
        
        PaginatedList<Facility> pagedFacilities = await facilityQuery
            .OrderByDescending(f => f.CreatedAt)
            .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

        if (pagedFacilities.Items.Count is 0)
        {
            return new PaginatedList<FacilityDto>([], pagedFacilities.TotalCount, query.PageNumber, query.PageSize);
        }
        
        List<FacilityDto> facilitiesDto = MapToDtoList(pagedFacilities.Items);

        return new PaginatedList<FacilityDto>(facilitiesDto, pagedFacilities.TotalCount, query.PageNumber, query.PageSize);
    }

    private static List<FacilityDto> MapToDtoList(IEnumerable<Facility> facilities)
    {
        return facilities.Select(f => new FacilityDto(
            f.Id,
            f.Name,
            f.Status.ToString(),
            f.Address.City
        )).ToList();
    }
}