using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.ReturnRequest.GetMyReturnRequestById;

using ReturnRequestEntity = Models.Commerce.ReturnRequest;

public sealed class GetMyReturnRequestByIdQueryHandler : IQueryHandler<GetMyReturnRequestByIdQuery, ReturnRequestDetailDto>
{
    private readonly IReturnRequestRepository _returnRequestRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyReturnRequestByIdQueryHandler(IReturnRequestRepository returnRequestRepository, ICurrentUserService currentUserService)
    {
        _returnRequestRepository = returnRequestRepository;
        _currentUserService = currentUserService;
    }

    public async Task<ReturnRequestDetailDto> Handle(GetMyReturnRequestByIdQuery query, CancellationToken cancellationToken)
    {
        ReturnRequestEntity? returnRequest = await _returnRequestRepository.GetByIdWithLinesAsync(query.ReturnRequestId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(returnRequest, ErrorMessages.ReturnRequest.EntityName, query.ReturnRequestId);

        ForbiddenAccessException.ThrowIfFalse(
            returnRequest!.UserId == _currentUserService.UserId,
            ErrorMessages.ReturnRequest.AccessDenied,
            ErrorMessages.Exception.UnauthorizedAccess);

        return CommerceDtoMapper.ToReturnRequestDetailDto(returnRequest);
    }
}
