using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Features.PurchaseOrder.GetAdminPurchaseOrders;

public sealed record GetAdminPurchaseOrdersQuery(
    PurchaseOrderStatus? Status = null,
    string? UserId = null,
    DateTimeOffset? StartDate = null,
    DateTimeOffset? EndDate = null,
    OrderSource? OrderSource = null,
    Guid? ProductListingId = null,
    Guid? ProductId = null,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PaginatedList<AdminOrderSummaryDto>>, IPaginatedQuery;
