using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.UserSaleRequest.GetAdminUserSaleRequestById;

public sealed class GetAdminUserSaleRequestByIdQueryHandler : IQueryHandler<GetAdminUserSaleRequestByIdQuery, UserSaleRequestDto>
{
    private readonly IUserSaleRequestRepository _userSaleRequestRepository;

    public GetAdminUserSaleRequestByIdQueryHandler(IUserSaleRequestRepository userSaleRequestRepository)
    {
        _userSaleRequestRepository = userSaleRequestRepository;
    }

    public async Task<UserSaleRequestDto> Handle(GetAdminUserSaleRequestByIdQuery query, CancellationToken cancellationToken)
    {
        Models.Commerce.UserSaleRequest? request = await _userSaleRequestRepository.GetByIdAsync(query.RequestId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(request, ErrorMessages.UserSaleRequest.EntityName, query.RequestId);
        return CommerceDtoMapper.ToUserSaleRequestDto(request!);
    }
}
