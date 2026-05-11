using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Dispatch.Requests;

namespace E_Satis_Auction.Features.Dispatch.ReceiveDispatch;

public sealed record ReceiveDispatchCommand(
    Guid DispatchId,
    ReceiveDispatchRequest Payload) : IAuditableCommand;