using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce.Requests;

namespace E_Satis_Auction.Features.ReturnRequest.RejectReturnRequest;

public sealed record RejectReturnRequestCommand : IAuditableCommand
{
    public Guid ReturnRequestId { get; }
    public RejectReturnRequestRequest Payload { get; }

    public RejectReturnRequestCommand(Guid returnRequestId, RejectReturnRequestRequest payload)
    {
        ReturnRequestId = returnRequestId;
        Payload = payload;
    }
}
