using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;

namespace E_Satis_Auction.Features.ReturnRequest.GetAdminReturnRequestById;

public sealed record GetAdminReturnRequestByIdQuery(Guid ReturnRequestId) : IQuery<ReturnRequestDetailDto>;
