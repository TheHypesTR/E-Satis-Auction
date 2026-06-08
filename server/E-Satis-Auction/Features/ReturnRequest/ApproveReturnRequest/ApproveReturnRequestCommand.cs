using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce.Requests;

namespace E_Satis_Auction.Features.ReturnRequest.ApproveReturnRequest;

public sealed record ApproveReturnRequestCommand : IAuditableCommand
{
    public Guid ReturnRequestId { get; }
    public ApproveReturnRequestRequest Payload { get; }

    public ApproveReturnRequestCommand(Guid returnRequestId, ApproveReturnRequestRequest payload)
    {
        ReturnRequestId = returnRequestId;
        Payload = payload;
    }
}
