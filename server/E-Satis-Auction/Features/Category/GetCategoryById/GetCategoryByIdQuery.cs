using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Category;

namespace E_Satis_Auction.Features.Category.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid Id) : ICacheableQuery<CategoryDetailDto>
{
	public string CacheKey => CacheKeys.GetCategoryById(Id);
	public TimeSpan? Expiration => TimeSpan.FromHours(1);
	public bool BypassCache => false;
}