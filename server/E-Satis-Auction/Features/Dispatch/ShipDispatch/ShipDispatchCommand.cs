using E_Satis_Auction.Common.Interfaces.Messaging;

namespace E_Satis_Auction.Features.Dispatch.ShipDispatch;

public sealed record ShipDispatchCommand(Guid DispatchId) : IAuditableCommand;