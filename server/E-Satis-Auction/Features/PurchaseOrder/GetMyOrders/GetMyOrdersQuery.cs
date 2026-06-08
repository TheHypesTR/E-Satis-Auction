using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Features.PurchaseOrder.GetMyOrders;

public sealed record GetMyOrdersQuery(
    PurchaseOrderStatus? Status = null,
    OrderSource? OrderSource = null,
    DateTimeOffset? StartDate = null,
    DateTimeOffset? EndDate = null,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PaginatedList<OrderSummaryDto>>, IPaginatedQuery;
