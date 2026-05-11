using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Facility;
using E_Satis_Auction.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Features.Facility.GetAllFacilities;

using Models.Facilities;

public class GetAllFacilitiesQueryHandler : IQueryHandler<GetAllFacilitiesQuery, PaginatedList<FacilityDto>>
{
    private readonly IFacilityRepository _facilityRepository;

    public GetAllFacilitiesQueryHandler(IFacilityRepository facilityRepository)
    {
        _facilityRepository = facilityRepository;
    }

    public async Task<PaginatedList<FacilityDto>> Handle(GetAllFacilitiesQuery query,CancellationToken cancellationToken)
    {
        IQueryable<Facility> facilityQuery = _facilityRepository.GetAllAsQueryable().Include(f => f.Address);
        facilityQuery = ApplyFilters(facilityQuery, query);
        
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

    private static IQueryable<Facility> ApplyFilters(IQueryable<Facility> query, GetAllFacilitiesQuery filters)
    {
        if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
        {
            string search = filters.SearchTerm.ToLower();
            query = query.Where(f =>
                f.Name.ToLower().Contains(search) ||
                f.Description.ToLower().Contains(search)
            );
        }

        if (filters.Status.HasValue)
        {
            query = query.Where(f => f.Status == filters.Status.Value);
        }

        if (filters.OrganizationId.HasValue)
        {
            query = query.Where(f => f.OrganizationId == filters.OrganizationId.Value);
        }
        
        if (!string.IsNullOrWhiteSpace(filters.City))
        {
            string city = filters.City.ToLower();
            query = query.Where(f => f.Address.City.ToLower().Contains(city)); 
        }

        return query;
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