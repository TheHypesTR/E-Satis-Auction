using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Product;

namespace E_Satis_Auction.Features.Product.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) : ICacheableQuery<ProductDetailDto>
{
    public string CacheKey => CacheKeys.GetProductById(Id);
    public TimeSpan? Expiration => TimeSpan.FromMinutes(15);
    public bool BypassCache => false;
};