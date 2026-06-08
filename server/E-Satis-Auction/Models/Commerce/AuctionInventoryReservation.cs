using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Models.Commerce;

public sealed class AuctionInventoryReservation : BaseEntity
{
    public Guid AuctionId { get; private set; }
    public Guid OriginalItemId { get; private set; }
    public Guid ReservedItemId { get; private set; }
    public int Quantity { get; private set; }
    public AuctionReservationStatus Status { get; private set; }

    private AuctionInventoryReservation()
    {
        Status = AuctionReservationStatus.Active;
    }

    public static AuctionInventoryReservation Create(Guid auctionId, Guid originalItemId, Guid reservedItemId, int quantity)
    {
        BusinessException.ThrowIfTrue(auctionId == Guid.Empty, ErrorMessages.Auction.EntityName, ErrorMessages.Exception.CommerceTitle);
        BusinessException.ThrowIfTrue(originalItemId == Guid.Empty, ErrorMessages.PurchaseOrder.OriginalItemRequired, ErrorMessages.Exception.CommerceTitle);
        BusinessException.ThrowIfTrue(reservedItemId == Guid.Empty, ErrorMessages.PurchaseOrder.ReservedItemRequired, ErrorMessages.Exception.CommerceTitle);
        BusinessException.ThrowIfTrue(quantity <= 0, ErrorMessages.PurchaseOrder.QuantityMustBePositive, ErrorMessages.Exception.CommerceTitle);

        return new AuctionInventoryReservation
        {
            AuctionId = auctionId,
            OriginalItemId = originalItemId,
            ReservedItemId = reservedItemId,
            Quantity = quantity,
            Status = AuctionReservationStatus.Active
        };
    }

    public void MarkReleased()
    {
        Status = AuctionReservationStatus.Released;
    }

    public void MarkTransferredToOrder()
    {
        Status = AuctionReservationStatus.TransferredToOrder;
    }
}
