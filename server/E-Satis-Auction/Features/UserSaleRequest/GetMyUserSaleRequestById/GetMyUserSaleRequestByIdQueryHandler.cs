using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.UserSaleRequest.GetMyUserSaleRequestById;

public sealed class GetMyUserSaleRequestByIdQueryHandler : IQueryHandler<GetMyUserSaleRequestByIdQuery, UserSaleRequestDto>
{
    private readonly IUserSaleRequestRepository _userSaleRequestRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyUserSaleRequestByIdQueryHandler(IUserSaleRequestRepository userSaleRequestRepository, ICurrentUserService currentUserService)
    {
        _userSaleRequestRepository = userSaleRequestRepository;
        _currentUserService = currentUserService;
    }

    public async Task<UserSaleRequestDto> Handle(GetMyUserSaleRequestByIdQuery query, CancellationToken cancellationToken)
    {
        Models.Commerce.UserSaleRequest? request = await _userSaleRequestRepository.GetByIdAsync(query.RequestId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(request, ErrorMessages.UserSaleRequest.EntityName, query.RequestId);
        ForbiddenAccessException.ThrowIfTrue(request!.UserId != _currentUserService.UserId, ErrorMessages.UserSaleRequest.AccessDenied, ErrorMessages.Exception.UnauthorizedAccess);
        return CommerceDtoMapper.ToUserSaleRequestDto(request);
    }
}
