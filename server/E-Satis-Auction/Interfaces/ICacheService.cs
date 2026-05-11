namespace E_Satis_Auction.Interfaces;

public interface ICacheService
{
    Task RemoveAsync(string cacheKey, CancellationToken cancellationToken = default);
}