using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce.Requests;

namespace E_Satis_Auction.Features.PurchaseOrder.ApprovePurchaseOrder;

public sealed record ApprovePurchaseOrderCommand : IAuditableCommand
{
    public Guid PurchaseOrderId { get; }
    public ApprovePurchaseOrderRequest Payload { get; }

    public ApprovePurchaseOrderCommand(Guid purchaseOrderId, ApprovePurchaseOrderRequest payload)
    {
        PurchaseOrderId = purchaseOrderId;
        Payload = payload;
    }
}
