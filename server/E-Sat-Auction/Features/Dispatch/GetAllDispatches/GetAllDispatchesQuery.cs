using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Common.Models;
using e_Sat_Auction.Dtos.Dispatch;
using e_Sat_Auction.Enums;

namespace e_Sat_Auction.Features.Dispatch.GetAllDispatches;

public sealed record GetAllDispatchesQuery(
    string? SearchTerm = null,
    Guid? SourceFacilityId = null,
    Guid? TargetFacilityId = null,
    DispatchStatus? Status = null,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PaginatedList<DispatchSummaryDto>>, IPaginatedQuery;