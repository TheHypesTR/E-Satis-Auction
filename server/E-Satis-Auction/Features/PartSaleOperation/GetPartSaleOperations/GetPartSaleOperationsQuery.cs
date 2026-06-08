using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Commerce;

namespace E_Satis_Auction.Features.PartSaleOperation.GetPartSaleOperations;

public sealed record GetPartSaleOperationsQuery(int PageNumber = 1, int PageSize = 10)
    : IQuery<PaginatedList<PartSaleOperationDto>>, IPaginatedQuery;
