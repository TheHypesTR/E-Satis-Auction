using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce.Requests;

namespace E_Satis_Auction.Features.PurchaseOrder.RejectPurchaseOrder;

public sealed record RejectPurchaseOrderCommand : IAuditableCommand
{
    public Guid PurchaseOrderId { get; }
    public RejectOrderRequest Payload { get; }

    public RejectPurchaseOrderCommand(Guid purchaseOrderId, RejectOrderRequest payload)
    {
        PurchaseOrderId = purchaseOrderId;
        Payload = payload;
    }
}
