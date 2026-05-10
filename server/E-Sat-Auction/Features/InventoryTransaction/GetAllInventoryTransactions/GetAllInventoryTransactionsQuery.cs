using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Common.Models;
using e_Sat_Auction.Dtos.InventoryTransaction;
using e_Sat_Auction.Enums;

namespace e_Sat_Auction.Features.InventoryTransaction.GetAllInventoryTransactions;

public sealed record GetAllInventoryTransactionsQuery(
    Guid? FacilityId = null,
    Guid? ItemId = null,
    InventoryTransactionType? TransactionType = null,
    Guid? ReferenceId = null,
    DateTimeOffset? StartDate = null,
    DateTimeOffset? EndDate = null,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PaginatedList<InventoryTransactionDto>>, IPaginatedQuery;