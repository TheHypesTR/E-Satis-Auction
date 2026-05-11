using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Item;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Features.Item.GetAllItems;

public sealed record GetAllItemsQuery(
    string? SearchTerm = null,
    ItemStatus? Status = null,
    ItemMode? Mode = null,
    Guid? FacilityId = null,
    Guid? CategoryId = null,
    Guid? ProductId = null,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PaginatedList<ItemSummaryDto>>, IPaginatedQuery;