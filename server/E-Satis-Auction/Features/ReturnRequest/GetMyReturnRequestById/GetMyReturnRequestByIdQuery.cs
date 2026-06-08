using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;

namespace E_Satis_Auction.Features.ReturnRequest.GetMyReturnRequestById;

public sealed record GetMyReturnRequestByIdQuery(Guid ReturnRequestId) : IQuery<ReturnRequestDetailDto>;
