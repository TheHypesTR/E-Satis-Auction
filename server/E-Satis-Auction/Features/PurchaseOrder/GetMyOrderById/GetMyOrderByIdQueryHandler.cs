using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.PurchaseOrder.GetMyOrderById;

using PurchaseOrderEntity = Models.Commerce.PurchaseOrder;
using ReturnRequestEntity = Models.Commerce.ReturnRequest;

public sealed class GetMyOrderByIdQueryHandler : IQueryHandler<GetMyOrderByIdQuery, OrderDetailDto>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IReturnRequestRepository _returnRequestRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyOrderByIdQueryHandler(
        IPurchaseOrderRepository purchaseOrderRepository,
        IReturnRequestRepository returnRequestRepository,
        ICurrentUserService currentUserService)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _returnRequestRepository = returnRequestRepository;
        _currentUserService = currentUserService;
    }

    public async Task<OrderDetailDto> Handle(GetMyOrderByIdQuery query, CancellationToken cancellationToken)
    {
        PurchaseOrderEntity? order = await _purchaseOrderRepository.GetByIdWithDetailsAsync(query.PurchaseOrderId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(order, ErrorMessages.PurchaseOrder.EntityName, query.PurchaseOrderId);

        ForbiddenAccessException.ThrowIfFalse(
            order!.UserId == _currentUserService.UserId,
            ErrorMessages.PurchaseOrder.AccessDenied,
            ErrorMessages.Exception.UnauthorizedAccess);

        List<ReturnRequestSummaryDto> returnRequests = _returnRequestRepository
            .GetAllAsQueryable()
            .Where(returnRequest => returnRequest.PurchaseOrderId == order.Id)
            .OrderByDescending(returnRequest => returnRequest.CreatedAt)
            .AsEnumerable()
            .Select(CommerceDtoMapper.ToReturnRequestSummaryDto)
            .ToList();

        return CommerceDtoMapper.ToOrderDetailDto(order, returnRequests);
    }
}
