using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Features.ReturnRequest.GetAdminReturnRequests;

using ReturnRequestEntity = Models.Commerce.ReturnRequest;

public sealed class GetAdminReturnRequestsQueryHandler : IQueryHandler<GetAdminReturnRequestsQuery, PaginatedList<AdminReturnRequestSummaryDto>>
{
    private readonly IReturnRequestRepository _returnRequestRepository;
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly UserManager<AppUser> _userManager;

    public GetAdminReturnRequestsQueryHandler(
        IReturnRequestRepository returnRequestRepository,
        IPurchaseOrderRepository purchaseOrderRepository,
        UserManager<AppUser> userManager)
    {
        _returnRequestRepository = returnRequestRepository;
        _purchaseOrderRepository = purchaseOrderRepository;
        _userManager = userManager;
    }

    public async Task<PaginatedList<AdminReturnRequestSummaryDto>> Handle(GetAdminReturnRequestsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<ReturnRequestEntity> returnRequestQuery = ApplyFilters(_returnRequestRepository.GetAllAsQueryable(), query);

        PaginatedList<ReturnRequestEntity> pagedReturnRequests = await returnRequestQuery
            .OrderByDescending(returnRequest => returnRequest.CreatedAt)
            .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

        Dictionary<Guid, string> orderNumbers = await GetOrderNumbersAsync(pagedReturnRequests.Items.Select(request => request.PurchaseOrderId), cancellationToken);
        Dictionary<string, string> userNames = await GetUserDisplayNamesAsync(pagedReturnRequests.Items.Select(request => request.UserId), cancellationToken);

        List<AdminReturnRequestSummaryDto> dtoList = pagedReturnRequests.Items
            .Select(returnRequest => CommerceDtoMapper.ToAdminReturnRequestSummaryDto(
                returnRequest,
                orderNumbers.GetValueOrDefault(returnRequest.PurchaseOrderId, string.Empty),
                userNames.GetValueOrDefault(returnRequest.UserId, returnRequest.UserId)))
            .ToList();

        return new PaginatedList<AdminReturnRequestSummaryDto>(dtoList, pagedReturnRequests.TotalCount, pagedReturnRequests.PageNumber, query.PageSize);
    }

    private static IQueryable<ReturnRequestEntity> ApplyFilters(IQueryable<ReturnRequestEntity> returnRequestQuery, GetAdminReturnRequestsQuery query)
    {
        if (query.Status.HasValue)
        {
            returnRequestQuery = returnRequestQuery.Where(returnRequest => returnRequest.Status == query.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.UserId))
        {
            returnRequestQuery = returnRequestQuery.Where(returnRequest => returnRequest.UserId == query.UserId);
        }

        if (query.PurchaseOrderId.HasValue)
        {
            returnRequestQuery = returnRequestQuery.Where(returnRequest => returnRequest.PurchaseOrderId == query.PurchaseOrderId.Value);
        }

        if (query.StartDate.HasValue)
        {
            returnRequestQuery = returnRequestQuery.Where(returnRequest => returnRequest.CreatedAt >= query.StartDate.Value.UtcDateTime);
        }

        if (query.EndDate.HasValue)
        {
            returnRequestQuery = returnRequestQuery.Where(returnRequest => returnRequest.CreatedAt <= query.EndDate.Value.UtcDateTime);
        }

        return returnRequestQuery;
    }

    private async Task<Dictionary<Guid, string>> GetOrderNumbersAsync(IEnumerable<Guid> orderIds, CancellationToken cancellationToken)
    {
        List<Guid> ids = orderIds.Distinct().ToList();
        if (ids.Count is 0)
        {
            return [];
        }

        return await _purchaseOrderRepository.GetAllAsQueryable()
            .Where(order => ids.Contains(order.Id))
            .ToDictionaryAsync(order => order.Id, order => order.OrderNumber, cancellationToken);
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
