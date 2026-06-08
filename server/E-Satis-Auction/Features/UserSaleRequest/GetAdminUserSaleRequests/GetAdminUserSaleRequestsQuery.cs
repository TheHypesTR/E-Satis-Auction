using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Features.UserSaleRequest.GetAdminUserSaleRequests;

public sealed record GetAdminUserSaleRequestsQuery(UserSaleRequestStatus? Status = null, int PageNumber = 1, int PageSize = 10)
    : IQuery<PaginatedList<UserSaleRequestDto>>, IPaginatedQuery;
