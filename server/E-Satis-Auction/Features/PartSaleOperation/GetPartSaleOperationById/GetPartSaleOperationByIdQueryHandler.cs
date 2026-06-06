using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.PartSaleOperation.GetPartSaleOperationById;

public sealed class GetPartSaleOperationByIdQueryHandler : IQueryHandler<GetPartSaleOperationByIdQuery, PartSaleOperationDto>
{
    private readonly IPartSaleOperationRepository _partSaleOperationRepository;

    public GetPartSaleOperationByIdQueryHandler(IPartSaleOperationRepository partSaleOperationRepository)
    {
        _partSaleOperationRepository = partSaleOperationRepository;
    }

    public async Task<PartSaleOperationDto> Handle(GetPartSaleOperationByIdQuery query, CancellationToken cancellationToken)
    {
        Models.Commerce.PartSaleOperation? operation = await _partSaleOperationRepository.GetByIdAsync(query.OperationId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(operation, ErrorMessages.PartSaleOperation.EntityName, query.OperationId);
        return CommerceDtoMapper.ToPartSaleOperationDto(operation!);
    }
}
