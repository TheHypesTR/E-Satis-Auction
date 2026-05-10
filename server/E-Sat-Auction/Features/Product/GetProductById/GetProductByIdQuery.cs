using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.Product;

namespace e_Sat_Auction.Features.Product.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) : ICacheableQuery<ProductDetailDto>
{
    public string CacheKey => CacheKeys.GetProductById(Id);
    public TimeSpan? Expiration => TimeSpan.FromMinutes(15);
    public bool BypassCache => false;
};