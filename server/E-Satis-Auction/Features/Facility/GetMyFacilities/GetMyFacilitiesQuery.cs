using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Facility;

namespace E_Satis_Auction.Features.Facility.GetMyFacilities;

public record GetMyFacilitiesQuery(int PageNumber = 1, int PageSize = 10) :
    IQuery<PaginatedList<FacilityDto>>, IPaginatedQuery;