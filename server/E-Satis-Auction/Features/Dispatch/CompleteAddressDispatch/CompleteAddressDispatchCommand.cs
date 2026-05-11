using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Dispatch.Requests;

namespace E_Satis_Auction.Features.Dispatch.CompleteAddressDispatch;

public sealed record CompleteAddressDispatchCommand(
    Guid DispatchId,
    CompleteAddressDispatchRequest Payload) : IAuditableCommand;