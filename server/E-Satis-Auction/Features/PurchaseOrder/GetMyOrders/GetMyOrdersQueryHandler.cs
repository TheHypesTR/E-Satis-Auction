using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.PurchaseOrder.GetMyOrders;

using PurchaseOrderEntity = Models.Commerce.PurchaseOrder;

public sealed class GetMyOrdersQueryHandler : IQueryHandler<GetMyOrdersQuery, PaginatedList<OrderSummaryDto>>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyOrdersQueryHandler(IPurchaseOrderRepository purchaseOrderRepository, ICurrentUserService currentUserService)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedList<OrderSummaryDto>> Handle(GetMyOrdersQuery query, CancellationToken cancellationToken)
    {
        IQueryable<PurchaseOrderEntity> orderQuery = _purchaseOrderRepository
            .GetAllAsQueryable()
            .Where(order => order.UserId == _currentUserService.UserId);

        orderQuery = ApplyFilters(orderQuery, query);

        PaginatedList<PurchaseOrderEntity> pagedOrders = await orderQuery
            .OrderByDescending(order => order.CreatedAt)
            .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

        List<OrderSummaryDto> dtoList = pagedOrders.Items
            .Select(CommerceDtoMapper.ToOrderSummaryDto)
            .ToList();

        return new PaginatedList<OrderSummaryDto>(dtoList, pagedOrders.TotalCount, pagedOrders.PageNumber, query.PageSize);
    }

    private static IQueryable<PurchaseOrderEntity> ApplyFilters(IQueryable<PurchaseOrderEntity> orderQuery, GetMyOrdersQuery query)
    {
        if (query.Status.HasValue)
        {
            orderQuery = orderQuery.Where(order => order.Status == query.Status.Value);
        }

        if (query.OrderSource.HasValue)
        {
            orderQuery = orderQuery.Where(order => order.OrderSource == query.OrderSource.Value);
        }

        if (query.StartDate.HasValue)
        {
            orderQuery = orderQuery.Where(order => order.CreatedAt >= query.StartDate.Value.UtcDateTime);
        }

        if (query.EndDate.HasValue)
        {
            orderQuery = orderQuery.Where(order => order.CreatedAt <= query.EndDate.Value.UtcDateTime);
        }

        return orderQuery;
    }
}
