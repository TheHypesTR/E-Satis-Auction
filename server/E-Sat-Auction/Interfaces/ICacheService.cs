namespace e_Sat_Auction.Interfaces;

public interface ICacheService
{
    Task RemoveAsync(string cacheKey, CancellationToken cancellationToken = default);
}