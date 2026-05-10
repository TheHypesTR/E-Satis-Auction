using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.Item;

namespace e_Sat_Auction.Features.Item.GetItemById;

public sealed record GetItemByIdQuery(Guid Id) : IQuery<ItemDetailDto>;