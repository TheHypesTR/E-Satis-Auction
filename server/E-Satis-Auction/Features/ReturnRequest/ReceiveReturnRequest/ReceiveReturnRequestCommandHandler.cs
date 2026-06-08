using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.ReturnRequest.ReceiveReturnRequest;

using ItemEntity = Models.Items.Item;
using PurchaseOrderEntity = Models.Commerce.PurchaseOrder;
using PurchaseOrderLineEntity = Models.Commerce.PurchaseOrderLine;
using ReturnRequestEntity = Models.Commerce.ReturnRequest;
using ReturnRequestLineEntity = Models.Commerce.ReturnRequestLine;
using ReturnRequestLineReceiveInfoEntity = Models.Commerce.ReturnRequestLineReceiveInfo;

public sealed class ReceiveReturnRequestCommandHandler : ICommandHandler<ReceiveReturnRequestCommand, ReturnRequestDetailDto>
{
    private readonly IReturnRequestRepository _returnRequestRepository;
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ReceiveReturnRequestCommandHandler(
        IReturnRequestRepository returnRequestRepository,
        IPurchaseOrderRepository purchaseOrderRepository,
        IItemRepository itemRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _returnRequestRepository = returnRequestRepository;
        _purchaseOrderRepository = purchaseOrderRepository;
        _itemRepository = itemRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ReturnRequestDetailDto> Handle(ReceiveReturnRequestCommand command, CancellationToken cancellationToken)
    {
        string adminUserId = _currentUserService.UserId;
        ForbiddenAccessException.ThrowIfTrue(
            string.IsNullOrWhiteSpace(adminUserId),
            ErrorMessages.Auth.UnauthorizedAccess,
            ErrorMessages.Exception.UnauthorizedAccess);

        ReturnRequestEntity? returnRequest = await _returnRequestRepository.GetByIdWithLinesAsync(
            command.ReturnRequestId,
            enableTracking: true,
            cancellationToken);
        NotFoundException.ThrowIfNull(returnRequest, ErrorMessages.ReturnRequest.EntityName, command.ReturnRequestId);

        PurchaseOrderEntity? order = await _purchaseOrderRepository.GetByIdWithDetailsAsync(
            returnRequest!.PurchaseOrderId,
            enableTracking: false,
            cancellationToken);
        NotFoundException.ThrowIfNull(order, ErrorMessages.PurchaseOrder.EntityName, returnRequest.PurchaseOrderId);

        List<ReturnLineReceivePlan> receivePlans = BuildReceivePlans(returnRequest, command.Payload.Lines);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            returnRequest.Receive(
                adminUserId,
                command.Payload.Note,
                receivePlans.Select(plan => new ReturnRequestLineReceiveInfoEntity(
                    plan.ReturnRequestLineId,
                    plan.ReceivedQuantity,
                    plan.RestockQuantity,
                    plan.Note)).ToList());

            await CreateReturnedInventoryAsync(
                returnRequest.Id,
                order!,
                receivePlans,
                command.Payload.TargetFacilityId,
                cancellationToken);

            _returnRequestRepository.Update(returnRequest);
            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return CommerceDtoMapper.ToReturnRequestDetailDto(returnRequest);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private static List<ReturnLineReceivePlan> BuildReceivePlans(
        ReturnRequestEntity returnRequest,
        IReadOnlyCollection<ReceiveReturnRequestLineRequest>? requestLines)
    {
        Dictionary<Guid, ReceiveReturnRequestLineRequest> requestLookup = requestLines?
            .ToDictionary(line => line.ReturnRequestLineId) ?? [];

        List<ReturnLineReceivePlan> plans = [];
        foreach (ReturnRequestLineEntity line in returnRequest.Lines)
        {
            if (requestLookup.TryGetValue(line.Id, out ReceiveReturnRequestLineRequest? requestLine))
            {
                plans.Add(new ReturnLineReceivePlan(
                    line.Id,
                    line.PurchaseOrderLineId,
                    requestLine.ReceivedQuantity,
                    requestLine.RestockQuantity,
                    requestLine.Note));

                continue;
            }

            plans.Add(new ReturnLineReceivePlan(
                line.Id,
                line.PurchaseOrderLineId,
                line.Quantity,
                line.Quantity,
                null));
        }

        foreach (Guid requestLineId in requestLookup.Keys)
        {
            ReturnRequestLineEntity? returnRequestLine = returnRequest.Lines.FirstOrDefault(line => line.Id == requestLineId);
            NotFoundException.ThrowIfNull(
                returnRequestLine,
                ErrorMessages.ReturnRequest.LineNotFound,
                requestLineId);
        }

        return plans;
    }

    private async Task CreateReturnedInventoryAsync(
        Guid returnRequestId,
        PurchaseOrderEntity order,
        IReadOnlyCollection<ReturnLineReceivePlan> receivePlans,
        Guid? targetFacilityId,
        CancellationToken cancellationToken)
    {
        Dictionary<Guid, PurchaseOrderLineEntity> orderLineLookup = order.Lines.ToDictionary(line => line.Id);
        List<Guid> originalItemIds = order.Lines
            .SelectMany(line => line.Allocations)
            .Select(allocation => allocation.OriginalItemId)
            .Distinct()
            .ToList();

        List<ItemEntity> originalItems = await _itemRepository.GetItemsByIdsAsync(originalItemIds, enableTracking: false, cancellationToken);
        Dictionary<Guid, ItemEntity> itemLookup = originalItems.ToDictionary(item => item.Id);

        foreach (ReturnLineReceivePlan plan in receivePlans)
        {
            orderLineLookup.TryGetValue(plan.PurchaseOrderLineId, out PurchaseOrderLineEntity? orderLine);
            NotFoundException.ThrowIfNull(orderLine, ErrorMessages.PurchaseOrder.LineRequired, plan.PurchaseOrderLineId);

            await CreateInventoryForLineAsync(
                returnRequestId,
                orderLine!,
                plan.ReceivedQuantity,
                plan.RestockQuantity,
                targetFacilityId,
                itemLookup,
                cancellationToken);
        }
    }

    private async Task CreateInventoryForLineAsync(
        Guid returnRequestId,
        PurchaseOrderLineEntity orderLine,
        int receivedQuantity,
        int restockQuantity,
        Guid? targetFacilityId,
        IReadOnlyDictionary<Guid, ItemEntity> itemLookup,
        CancellationToken cancellationToken)
    {
        int remainingReceived = receivedQuantity;
        int remainingRestock = restockQuantity;

        foreach (Models.Commerce.PurchaseOrderLineAllocation allocation in orderLine.Allocations)
        {
            if (remainingReceived is 0)
            {
                break;
            }

            itemLookup.TryGetValue(allocation.OriginalItemId, out ItemEntity? sourceItem);
            NotFoundException.ThrowIfNull(sourceItem, ErrorMessages.Item.EntityName, allocation.OriginalItemId);

            int allocationReceivedQuantity = Math.Min(allocation.Quantity, remainingReceived);
            int allocationRestockQuantity = Math.Min(allocationReceivedQuantity, remainingRestock);
            int allocationDamagedQuantity = allocationReceivedQuantity - allocationRestockQuantity;
            Guid facilityId = targetFacilityId ?? sourceItem!.FacilityId;

            if (allocationRestockQuantity > 0)
            {
                await CreateReturnedItemAsync(
                    orderLine.ProductId,
                    sourceItem!,
                    facilityId,
                    allocationRestockQuantity,
                    ItemStatus.Available,
                    InventoryTransactionType.PurchaseReturned,
                    returnRequestId,
                    cancellationToken);
            }

            if (allocationDamagedQuantity > 0)
            {
                await CreateReturnedItemAsync(
                    orderLine.ProductId,
                    sourceItem!,
                    facilityId,
                    allocationDamagedQuantity,
                    ItemStatus.Damaged,
                    InventoryTransactionType.Damaged,
                    returnRequestId,
                    cancellationToken);
            }

            remainingReceived -= allocationReceivedQuantity;
            remainingRestock -= allocationRestockQuantity;
        }

        BusinessException.ThrowIfTrue(
            remainingReceived > 0,
            ErrorMessages.ReturnRequest.InvalidReceiveQuantity,
            ErrorMessages.Exception.CommerceTitle);
    }

    private async Task CreateReturnedItemAsync(
        Guid productId,
        ItemEntity sourceItem,
        Guid facilityId,
        int quantity,
        ItemStatus status,
        InventoryTransactionType transactionType,
        Guid returnRequestId,
        CancellationToken cancellationToken)
    {
        Dictionary<string, string> attributes = sourceItem.DynamicAttributes
            .ToDictionary(entry => entry.Key, entry => entry.Value);

        ItemEntity returnedItem = ItemEntity.CreateFromProduct(
            productId,
            sourceItem.CategoryId,
            facilityId,
            quantity,
            sourceItem.UnitOfMeasure,
            status,
            attributes,
            transactionType,
            returnRequestId);

        await _itemRepository.AddAsync(returnedItem, cancellationToken);
    }

    private sealed record ReturnLineReceivePlan(
        Guid ReturnRequestLineId,
        Guid PurchaseOrderLineId,
        int ReceivedQuantity,
        int RestockQuantity,
        string? Note);
}
