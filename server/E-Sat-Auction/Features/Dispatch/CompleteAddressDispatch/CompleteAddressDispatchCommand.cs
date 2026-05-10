using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.Dispatch.Requests;

namespace e_Sat_Auction.Features.Dispatch.CompleteAddressDispatch;

public sealed record CompleteAddressDispatchCommand(
    Guid DispatchId,
    CompleteAddressDispatchRequest Payload) : IAuditableCommand;