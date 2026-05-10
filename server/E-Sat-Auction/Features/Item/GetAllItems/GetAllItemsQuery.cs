using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Common.Models;
using e_Sat_Auction.Dtos.Item;
using e_Sat_Auction.Enums;

namespace e_Sat_Auction.Features.Item.GetAllItems;

public sealed record GetAllItemsQuery(
    string? SearchTerm = null,
    ItemStatus? Status = null,
    ItemMode? Mode = null,
    Guid? FacilityId = null,
    Guid? CategoryId = null,
    Guid? ProductId = null,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PaginatedList<ItemSummaryDto>>, IPaginatedQuery;