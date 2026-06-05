using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Common.Exceptions;

namespace E_Satis_Auction.Models.Commerce;

public sealed class ReturnRequestLine : BaseEntity
{
    public Guid ReturnRequestId { get; private set; }
    public Guid PurchaseOrderLineId { get; private set; }
    public int Quantity { get; private set; }
    public string? Reason { get; private set; }
    public int ReceivedQuantity { get; private set; }
    public int RestockedQuantity { get; private set; }
    public string? ReceiveNote { get; private set; }

    private ReturnRequestLine()
    {
    }

    public static ReturnRequestLine Create(Guid returnRequestId, Guid purchaseOrderLineId, int quantity, string? reason)
    {
        BusinessException.ThrowIfTrue(
            returnRequestId == Guid.Empty,
            ErrorMessages.ReturnRequest.EntityName,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            purchaseOrderLineId == Guid.Empty,
            ErrorMessages.ReturnRequest.PurchaseOrderLineRequired,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            quantity <= 0,
            ErrorMessages.ReturnRequest.QuantityMustBePositive,
            ErrorMessages.Exception.CommerceTitle);

        return new ReturnRequestLine
        {
            ReturnRequestId = returnRequestId,
            PurchaseOrderLineId = purchaseOrderLineId,
            Quantity = quantity,
            Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim()
        };
    }

    public void Receive(int receivedQuantity, int restockedQuantity, string? receiveNote)
    {
        BusinessException.ThrowIfTrue(
            receivedQuantity <= 0,
            ErrorMessages.ReturnRequest.InvalidReceiveQuantity,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            receivedQuantity > Quantity,
            ErrorMessages.ReturnRequest.InvalidReceiveQuantity,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            restockedQuantity < 0 || restockedQuantity > receivedQuantity,
            ErrorMessages.ReturnRequest.InvalidRestockQuantity,
            ErrorMessages.Exception.CommerceTitle);

        ReceivedQuantity = receivedQuantity;
        RestockedQuantity = restockedQuantity;
        ReceiveNote = string.IsNullOrWhiteSpace(receiveNote) ? null : receiveNote.Trim();
    }
}
