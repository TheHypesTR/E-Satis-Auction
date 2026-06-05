using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Features.PurchaseOrder.GetAdminPurchaseOrderById;

using PurchaseOrderEntity = Models.Commerce.PurchaseOrder;

public sealed class GetAdminPurchaseOrderByIdQueryHandler : IQueryHandler<GetAdminPurchaseOrderByIdQuery, AdminOrderDetailDto>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IReturnRequestRepository _returnRequestRepository;
    private readonly UserManager<AppUser> _userManager;

    public GetAdminPurchaseOrderByIdQueryHandler(
        IPurchaseOrderRepository purchaseOrderRepository,
        IReturnRequestRepository returnRequestRepository,
        UserManager<AppUser> userManager)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _returnRequestRepository = returnRequestRepository;
        _userManager = userManager;
    }

    public async Task<AdminOrderDetailDto> Handle(GetAdminPurchaseOrderByIdQuery query, CancellationToken cancellationToken)
    {
        PurchaseOrderEntity? order = await _purchaseOrderRepository.GetByIdWithDetailsAsync(query.PurchaseOrderId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(order, ErrorMessages.PurchaseOrder.EntityName, query.PurchaseOrderId);

        List<ReturnRequestSummaryDto> returnRequests = _returnRequestRepository
            .GetAllAsQueryable()
            .Where(returnRequest => returnRequest.PurchaseOrderId == order!.Id)
            .OrderByDescending(returnRequest => returnRequest.CreatedAt)
            .AsEnumerable()
            .Select(CommerceDtoMapper.ToReturnRequestSummaryDto)
            .ToList();

        string userDisplayName = await GetUserDisplayNameAsync(order!.UserId, cancellationToken);
        return CommerceDtoMapper.ToAdminOrderDetailDto(order, userDisplayName, returnRequests);
    }

    private async Task<string> GetUserDisplayNameAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new { DisplayName = $"{u.FirstName} {u.LastName}" })
            .FirstOrDefaultAsync(cancellationToken);

        return user?.DisplayName ?? userId;
    }
}
