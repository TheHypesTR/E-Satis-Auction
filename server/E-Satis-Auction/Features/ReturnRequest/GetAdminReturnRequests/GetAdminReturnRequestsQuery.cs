using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Features.ReturnRequest.GetAdminReturnRequests;

public sealed record GetAdminReturnRequestsQuery(
    ReturnRequestStatus? Status = null,
    string? UserId = null,
    Guid? PurchaseOrderId = null,
    DateTimeOffset? StartDate = null,
    DateTimeOffset? EndDate = null,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PaginatedList<AdminReturnRequestSummaryDto>>, IPaginatedQuery;
