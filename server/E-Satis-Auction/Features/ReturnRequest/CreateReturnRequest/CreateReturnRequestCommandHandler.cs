using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.ReturnRequest.CreateReturnRequest;

using PurchaseOrderEntity = Models.Commerce.PurchaseOrder;
using ReturnRequestEntity = Models.Commerce.ReturnRequest;

public sealed class CreateReturnRequestCommandHandler : ICommandHandler<CreateReturnRequestCommand, ReturnRequestDetailDto>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IReturnRequestRepository _returnRequestRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateReturnRequestCommandHandler(
        IPurchaseOrderRepository purchaseOrderRepository,
        IReturnRequestRepository returnRequestRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _returnRequestRepository = returnRequestRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ReturnRequestDetailDto> Handle(CreateReturnRequestCommand command, CancellationToken cancellationToken)
    {
        PurchaseOrderEntity? order = await _purchaseOrderRepository.GetByIdWithDetailsAsync(command.PurchaseOrderId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(order, ErrorMessages.PurchaseOrder.EntityName, command.PurchaseOrderId);

        string userId = _currentUserService.UserId;
        ForbiddenAccessException.ThrowIfFalse(
            order!.UserId == userId,
            ErrorMessages.PurchaseOrder.AccessDenied,
            ErrorMessages.Exception.UnauthorizedAccess);

        BusinessException.ThrowIfFalse(
            order.Status is PurchaseOrderStatus.Shipped or PurchaseOrderStatus.Delivered,
            ErrorMessages.ReturnRequest.NotEligible,
            ErrorMessages.Exception.CommerceTitle);

        Dictionary<Guid, int> alreadyRequestedQuantities = GetExistingReturnQuantities(order.Id);
        Dictionary<Guid, Models.Commerce.PurchaseOrderLine> orderLineLookup = order.Lines.ToDictionary(line => line.Id, line => line);

        ReturnRequestEntity returnRequest = ReturnRequestEntity.Create(order.Id, userId, command.Payload.Reason);
        foreach (CreateReturnRequestLineRequest requestLine in command.Payload.Lines)
        {
            orderLineLookup.TryGetValue(requestLine.PurchaseOrderLineId, out Models.Commerce.PurchaseOrderLine? orderLine);
            NotFoundException.ThrowIfNull(orderLine, ErrorMessages.PurchaseOrder.LineRequired, requestLine.PurchaseOrderLineId);

            int alreadyRequested = alreadyRequestedQuantities.GetValueOrDefault(requestLine.PurchaseOrderLineId);
            BusinessException.ThrowIfTrue(
                requestLine.Quantity > orderLine!.Quantity - alreadyRequested,
                ErrorMessages.ReturnRequest.InvalidQuantity,
                ErrorMessages.Exception.CommerceTitle);

            returnRequest.AddLine(requestLine.PurchaseOrderLineId, requestLine.Quantity, requestLine.Reason);
        }

        await _returnRequestRepository.AddAsync(returnRequest, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return CommerceDtoMapper.ToReturnRequestDetailDto(returnRequest);
    }

    private Dictionary<Guid, int> GetExistingReturnQuantities(Guid purchaseOrderId)
    {
        return _returnRequestRepository
            .GetAllAsQueryable()
            .Where(returnRequest =>
                returnRequest.PurchaseOrderId == purchaseOrderId &&
                returnRequest.Status != ReturnRequestStatus.Rejected &&
                returnRequest.Status != ReturnRequestStatus.Cancelled)
            .SelectMany(returnRequest => returnRequest.Lines)
            .GroupBy(line => line.PurchaseOrderLineId)
            .ToDictionary(group => group.Key, group => group.Sum(line => line.Quantity));
    }
}
