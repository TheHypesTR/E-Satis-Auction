using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.UserSaleRequest.GetAdminUserSaleRequests;

public sealed class GetAdminUserSaleRequestsQueryHandler : IQueryHandler<GetAdminUserSaleRequestsQuery, PaginatedList<UserSaleRequestDto>>
{
    private readonly IUserSaleRequestRepository _userSaleRequestRepository;

    public GetAdminUserSaleRequestsQueryHandler(IUserSaleRequestRepository userSaleRequestRepository)
    {
        _userSaleRequestRepository = userSaleRequestRepository;
    }

    public async Task<PaginatedList<UserSaleRequestDto>> Handle(GetAdminUserSaleRequestsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Models.Commerce.UserSaleRequest> requestQuery = _userSaleRequestRepository.GetAllAsQueryable();
        if (query.Status.HasValue)
        {
            requestQuery = requestQuery.Where(request => request.Status == query.Status.Value);
        }

        PaginatedList<Models.Commerce.UserSaleRequest> paged = await requestQuery
            .OrderByDescending(request => request.CreatedAt)
            .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

        return new PaginatedList<UserSaleRequestDto>(
            paged.Items.Select(CommerceDtoMapper.ToUserSaleRequestDto).ToList(),
            paged.TotalCount,
            paged.PageNumber,
            query.PageSize);
    }
}
