using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce.Requests;

namespace E_Satis_Auction.Features.PurchaseOrder.ShipPurchaseOrder;

public sealed record ShipPurchaseOrderCommand : IAuditableCommand
{
    public Guid PurchaseOrderId { get; }
    public ShipOrderRequest Payload { get; }

    public ShipPurchaseOrderCommand(Guid purchaseOrderId, ShipOrderRequest payload)
    {
        PurchaseOrderId = purchaseOrderId;
        Payload = payload;
    }
}
