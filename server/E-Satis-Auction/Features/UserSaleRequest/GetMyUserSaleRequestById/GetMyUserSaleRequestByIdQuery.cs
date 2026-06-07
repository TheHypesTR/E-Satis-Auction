using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;

namespace E_Satis_Auction.Features.UserSaleRequest.GetMyUserSaleRequestById;

public sealed record GetMyUserSaleRequestByIdQuery(Guid RequestId) : IQuery<UserSaleRequestDto>;
