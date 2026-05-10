using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.Dispatch.Requests;

namespace e_Sat_Auction.Features.Dispatch.CreateDispatch;

public sealed record CreateDispatchCommand(
    Guid SourceFacilityId,
    CreateDispatchRequest Payload) : IAuditableCommand<Guid>;