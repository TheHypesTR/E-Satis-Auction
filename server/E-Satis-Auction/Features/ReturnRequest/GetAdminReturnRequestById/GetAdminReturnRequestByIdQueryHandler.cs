using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.ReturnRequest.GetAdminReturnRequestById;

using ReturnRequestEntity = Models.Commerce.ReturnRequest;

public sealed class GetAdminReturnRequestByIdQueryHandler : IQueryHandler<GetAdminReturnRequestByIdQuery, ReturnRequestDetailDto>
{
    private readonly IReturnRequestRepository _returnRequestRepository;

    public GetAdminReturnRequestByIdQueryHandler(IReturnRequestRepository returnRequestRepository)
    {
        _returnRequestRepository = returnRequestRepository;
    }

    public async Task<ReturnRequestDetailDto> Handle(GetAdminReturnRequestByIdQuery query, CancellationToken cancellationToken)
    {
        ReturnRequestEntity? returnRequest = await _returnRequestRepository.GetByIdWithLinesAsync(query.ReturnRequestId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(returnRequest, ErrorMessages.ReturnRequest.EntityName, query.ReturnRequestId);

        return CommerceDtoMapper.ToReturnRequestDetailDto(returnRequest!);
    }
}
