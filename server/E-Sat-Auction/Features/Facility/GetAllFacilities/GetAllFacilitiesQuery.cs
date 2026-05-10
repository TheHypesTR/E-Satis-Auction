using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Common.Models;
using e_Sat_Auction.Dtos.Facility;
using e_Sat_Auction.Enums;

namespace e_Sat_Auction.Features.Facility.GetAllFacilities;

public record GetAllFacilitiesQuery(
    string? SearchTerm,
    string? City,
    ApprovalStatus? Status,
    Guid? OrganizationId,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PaginatedList<FacilityDto>>, IPaginatedQuery;