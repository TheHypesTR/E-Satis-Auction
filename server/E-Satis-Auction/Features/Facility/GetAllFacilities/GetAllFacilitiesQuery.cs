using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Facility;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Features.Facility.GetAllFacilities;

public record GetAllFacilitiesQuery(
    string? SearchTerm,
    string? City,
    ApprovalStatus? Status,
    Guid? OrganizationId,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PaginatedList<FacilityDto>>, IPaginatedQuery;