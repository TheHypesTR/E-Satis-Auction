using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;

namespace E_Satis_Auction.Features.Cart.GetCart;

public sealed record GetCartQuery : IQuery<CartDto?>;
