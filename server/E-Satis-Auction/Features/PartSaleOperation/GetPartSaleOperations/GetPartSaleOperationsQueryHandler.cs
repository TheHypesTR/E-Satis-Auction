using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.PartSaleOperation.GetPartSaleOperations;

public sealed class GetPartSaleOperationsQueryHandler : IQueryHandler<GetPartSaleOperationsQuery, PaginatedList<PartSaleOperationDto>>
{
    private readonly IPartSaleOperationRepository _partSaleOperationRepository;

    public GetPartSaleOperationsQueryHandler(IPartSaleOperationRepository partSaleOperationRepository)
    {
        _partSaleOperationRepository = partSaleOperationRepository;
    }

    public async Task<PaginatedList<PartSaleOperationDto>> Handle(GetPartSaleOperationsQuery query, CancellationToken cancellationToken)
    {
        PaginatedList<Models.Commerce.PartSaleOperation> paged = await _partSaleOperationRepository
            .GetAllAsQueryable()
            .OrderByDescending(operation => operation.CreatedAt)
            .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

        return new PaginatedList<PartSaleOperationDto>(
            paged.Items.Select(CommerceDtoMapper.ToPartSaleOperationDto).ToList(),
            paged.TotalCount,
            paged.PageNumber,
            query.PageSize);
    }
}
