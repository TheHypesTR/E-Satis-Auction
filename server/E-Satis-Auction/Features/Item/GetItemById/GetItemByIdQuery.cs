using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Item;

namespace E_Satis_Auction.Features.Item.GetItemById;

public sealed record GetItemByIdQuery(Guid Id) : IQuery<ItemDetailDto>;