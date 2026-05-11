using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.InventoryTransaction;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Features.InventoryTransaction.GetAllInventoryTransactions;

public sealed record GetAllInventoryTransactionsQuery(
    Guid? FacilityId = null,
    Guid? ItemId = null,
    InventoryTransactionType? TransactionType = null,
    Guid? ReferenceId = null,
    DateTimeOffset? StartDate = null,
    DateTimeOffset? EndDate = null,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PaginatedList<InventoryTransactionDto>>, IPaginatedQuery;