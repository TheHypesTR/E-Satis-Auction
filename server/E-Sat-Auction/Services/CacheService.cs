using e_Sat_Auction.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace e_Sat_Auction.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }
    
    public async Task RemoveAsync(string cacheKey, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(cacheKey, cancellationToken);
            _logger.LogInformation("Cache invalidated for key: {CacheKey}", cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove cache for key: {CacheKey}", cacheKey);
        }
    }
}