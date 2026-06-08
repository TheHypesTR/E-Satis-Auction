using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;

namespace E_Satis_Auction.Features.ReturnRequest.CreateReturnRequest;

public sealed record CreateReturnRequestCommand : IAuditableCommand<ReturnRequestDetailDto>
{
    public Guid PurchaseOrderId { get; }
    public CreateReturnRequestRequest Payload { get; }

    public CreateReturnRequestCommand(Guid purchaseOrderId, CreateReturnRequestRequest payload)
    {
        PurchaseOrderId = purchaseOrderId;
        Payload = payload;
    }
}
