using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Common.Exceptions;

namespace E_Satis_Auction.Models.Commerce;

public sealed class PurchaseOrderLineAllocation : BaseEntity
{
    public Guid PurchaseOrderLineId { get; private set; }
    public Guid OriginalItemId { get; private set; }
    public Guid ReservedItemId { get; private set; }
    public int Quantity { get; private set; }

    private PurchaseOrderLineAllocation()
    {
    }

    public static PurchaseOrderLineAllocation Create(Guid purchaseOrderLineId, Guid originalItemId, Guid reservedItemId, int quantity)
    {
        BusinessException.ThrowIfTrue(
            purchaseOrderLineId == Guid.Empty,
            ErrorMessages.PurchaseOrder.LineRequired,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            originalItemId == Guid.Empty,
            ErrorMessages.PurchaseOrder.OriginalItemRequired,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            reservedItemId == Guid.Empty,
            ErrorMessages.PurchaseOrder.ReservedItemRequired,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            quantity <= 0,
            ErrorMessages.PurchaseOrder.QuantityMustBePositive,
            ErrorMessages.Exception.CommerceTitle);

        return new PurchaseOrderLineAllocation
        {
            PurchaseOrderLineId = purchaseOrderLineId,
            OriginalItemId = originalItemId,
            ReservedItemId = reservedItemId,
            Quantity = quantity
        };
    }
}
