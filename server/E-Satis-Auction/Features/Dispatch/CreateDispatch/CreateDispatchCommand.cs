using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Dispatch.Requests;

namespace E_Satis_Auction.Features.Dispatch.CreateDispatch;

public sealed record CreateDispatchCommand(
    Guid SourceFacilityId,
    CreateDispatchRequest Payload) : IAuditableCommand<Guid>;