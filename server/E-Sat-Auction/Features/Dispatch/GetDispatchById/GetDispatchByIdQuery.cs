using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.Dispatch;

namespace e_Sat_Auction.Features.Dispatch.GetDispatchById;

public sealed record GetDispatchByIdQuery(Guid Id) : IQuery<DispatchDetailDto>;