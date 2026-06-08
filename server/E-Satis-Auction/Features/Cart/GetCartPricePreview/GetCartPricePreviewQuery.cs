using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;

namespace E_Satis_Auction.Features.Cart.GetCartPricePreview;

public sealed record GetCartPricePreviewQuery : IQuery<CartPricePreviewDto>;
