using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.Dispatch.Requests;

namespace e_Sat_Auction.Features.Dispatch.CancelDispatch;

public sealed record CancelDispatchCommand(Guid DispatchId, CancelDispatchRequest Payload) : IAuditableCommand;