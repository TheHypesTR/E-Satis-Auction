using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Dispatch;

namespace E_Satis_Auction.Features.Dispatch.GetDispatchById;

public sealed record GetDispatchByIdQuery(Guid Id) : IQuery<DispatchDetailDto>;