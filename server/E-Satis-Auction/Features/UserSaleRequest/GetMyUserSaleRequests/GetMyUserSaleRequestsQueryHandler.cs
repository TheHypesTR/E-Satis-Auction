using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.UserSaleRequest.GetMyUserSaleRequests;

public sealed class GetMyUserSaleRequestsQueryHandler : IQueryHandler<GetMyUserSaleRequestsQuery, PaginatedList<UserSaleRequestDto>>
{
    private readonly IUserSaleRequestRepository _userSaleRequestRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyUserSaleRequestsQueryHandler(IUserSaleRequestRepository userSaleRequestRepository, ICurrentUserService currentUserService)
    {
        _userSaleRequestRepository = userSaleRequestRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedList<UserSaleRequestDto>> Handle(GetMyUserSaleRequestsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Models.Commerce.UserSaleRequest> requestQuery = _userSaleRequestRepository
            .GetAllAsQueryable()
            .Where(request => request.UserId == _currentUserService.UserId);

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
