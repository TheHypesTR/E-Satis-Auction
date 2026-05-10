using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Common.Models;
using e_Sat_Auction.Dtos.Facility;

namespace e_Sat_Auction.Features.Facility.GetMyFacilities;

public record GetMyFacilitiesQuery(int PageNumber = 1, int PageSize = 10) :
    IQuery<PaginatedList<FacilityDto>>, IPaginatedQuery;