using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.Dispatch.Requests;

namespace e_Sat_Auction.Features.Dispatch.ReceiveDispatch;

public sealed record ReceiveDispatchCommand(
    Guid DispatchId,
    ReceiveDispatchRequest Payload) : IAuditableCommand;