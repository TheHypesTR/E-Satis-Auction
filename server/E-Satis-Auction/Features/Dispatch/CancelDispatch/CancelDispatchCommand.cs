using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Dispatch.Requests;

namespace E_Satis_Auction.Features.Dispatch.CancelDispatch;

public sealed record CancelDispatchCommand(Guid DispatchId, CancelDispatchRequest Payload) : IAuditableCommand;