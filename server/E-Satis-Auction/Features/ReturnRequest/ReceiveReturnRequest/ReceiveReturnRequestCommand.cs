using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Dtos.Commerce.Requests;

namespace E_Satis_Auction.Features.ReturnRequest.ReceiveReturnRequest;

public sealed record ReceiveReturnRequestCommand : IAuditableCommand<ReturnRequestDetailDto>
{
    public Guid ReturnRequestId { get; }
    public ReceiveReturnRequestRequest Payload { get; }

    public ReceiveReturnRequestCommand(Guid returnRequestId, ReceiveReturnRequestRequest payload)
    {
        ReturnRequestId = returnRequestId;
        Payload = payload;
    }
}
