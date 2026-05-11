using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Dispatch;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Features.Dispatch.GetAllDispatches;

public sealed record GetAllDispatchesQuery(
    string? SearchTerm = null,
    Guid? SourceFacilityId = null,
    Guid? TargetFacilityId = null,
    DispatchStatus? Status = null,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PaginatedList<DispatchSummaryDto>>, IPaginatedQuery;