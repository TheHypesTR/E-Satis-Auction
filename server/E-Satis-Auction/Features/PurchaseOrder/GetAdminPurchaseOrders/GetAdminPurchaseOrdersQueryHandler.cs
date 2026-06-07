using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Features.PurchaseOrder.GetAdminPurchaseOrders;

using PurchaseOrderEntity = Models.Commerce.PurchaseOrder;

public sealed class GetAdminPurchaseOrdersQueryHandler : IQueryHandler<GetAdminPurchaseOrdersQuery, PaginatedList<AdminOrderSummaryDto>>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly UserManager<AppUser> _userManager;

    public GetAdminPurchaseOrdersQueryHandler(IPurchaseOrderRepository purchaseOrderRepository, UserManager<AppUser> userManager)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _userManager = userManager;
    }

    public async Task<PaginatedList<AdminOrderSummaryDto>> Handle(GetAdminPurchaseOrdersQuery query, CancellationToken cancellationToken)
    {
        IQueryable<PurchaseOrderEntity> orderQuery = ApplyFilters(_purchaseOrderRepository.GetAllAsQueryable(), query);

        PaginatedList<PurchaseOrderEntity> pagedOrders = await orderQuery
            .OrderByDescending(order => order.CreatedAt)
            .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

        Dictionary<string, string> userNames = await GetUserDisplayNamesAsync(pagedOrders.Items.Select(order => order.UserId), cancellationToken);
        List<AdminOrderSummaryDto> dtoList = pagedOrders.Items
            .Select(order => CommerceDtoMapper.ToAdminOrderSummaryDto(order, userNames.GetValueOrDefault(order.UserId, order.UserId)))
            .ToList();

        return new PaginatedList<AdminOrderSummaryDto>(dtoList, pagedOrders.TotalCount, pagedOrders.PageNumber, query.PageSize);
    }

    private static IQueryable<PurchaseOrderEntity> ApplyFilters(IQueryable<PurchaseOrderEntity> orderQuery, GetAdminPurchaseOrdersQuery query)
    {
        if (query.Status.HasValue)
        {
            orderQuery = orderQuery.Where(order => order.Status == query.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.UserId))
        {
            orderQuery = orderQuery.Where(order => order.UserId == query.UserId);
        }

        if (query.OrderSource.HasValue)
        {
            orderQuery = orderQuery.Where(order => order.OrderSource == query.OrderSource.Value);
        }

        if (query.ProductListingId.HasValue)
        {
            orderQuery = orderQuery.Where(order => order.Lines.Any(line => line.ProductListingId == query.ProductListingId.Value));
        }

        if (query.ProductId.HasValue)
        {
            orderQuery = orderQuery.Where(order => order.Lines.Any(line => line.ProductId == query.ProductId.Value));
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

    private async Task<Dictionary<string, string>> GetUserDisplayNamesAsync(IEnumerable<string> userIds, CancellationToken cancellationToken)
    {
        List<string> ids = userIds.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct().ToList();
        if (ids.Count is 0)
        {
            return [];
        }

        var users = await _userManager.Users
            .AsNoTracking()
            .Where(user => ids.Contains(user.Id))
            .Select(user => new { user.Id, DisplayName = $"{user.FirstName} {user.LastName}" })
            .ToListAsync(cancellationToken);

        return users.ToDictionary(user => user.Id, user => user.DisplayName);
    }
}
