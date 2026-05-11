namespace E_Satis_Auction.Common.Constants;

public static class CacheKeys
{
    public static string GetCategoryById(Guid categoryId) => $"Category_{categoryId}";
    public static string GetProductById(Guid productId) => $"Product_{productId}";
}