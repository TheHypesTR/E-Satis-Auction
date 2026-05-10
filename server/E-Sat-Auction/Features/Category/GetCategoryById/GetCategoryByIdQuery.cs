using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.Category;

namespace e_Sat_Auction.Features.Category.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid Id) : ICacheableQuery<CategoryDetailDto>
{
	public string CacheKey => CacheKeys.GetCategoryById(Id);
	public TimeSpan? Expiration => TimeSpan.FromHours(1);
	public bool BypassCache => false;
}