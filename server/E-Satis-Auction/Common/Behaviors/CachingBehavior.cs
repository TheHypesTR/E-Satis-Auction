using System.Text.Json;
using E_Satis_Auction.Common.Interfaces.Messaging;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace E_Satis_Auction.Common.Behaviors;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQueryMarker
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(IDistributedCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request.BypassCache)
        {
            _logger.LogInformation("Cache Bypass -> Key: {CacheKey}", request.CacheKey);
            TResponse response = await next(cancellationToken);
            await SetCacheAsync(request, response, cancellationToken);

            return response;
        }

        string? cachedResponse = await _cache.GetStringAsync(request.CacheKey, cancellationToken);
        if (!string.IsNullOrWhiteSpace(cachedResponse))
        {
            _logger.LogInformation("Fetching from Redis -> Key: {CacheKey}", request.CacheKey);
            return JsonSerializer.Deserialize<TResponse>(cachedResponse)!;
        }

        _logger.LogInformation("Fetching from Database -> Key: {CacheKey}", request.CacheKey);
        TResponse dbResponse = await next(cancellationToken);
        await SetCacheAsync(request, dbResponse, cancellationToken);

        return dbResponse;
    }

    private async Task SetCacheAsync(ICacheableQueryMarker query, TResponse response, CancellationToken cancellationToken)
    {
        if (response is null)
        {
            return;
        }

        DistributedCacheEntryOptions options = new()
        {
            AbsoluteExpirationRelativeToNow = query.Expiration ?? TimeSpan.FromDays(1)
        };
        string serializedData = JsonSerializer.Serialize(response);
        await _cache.SetStringAsync(query.CacheKey, serializedData, options, cancellationToken);
    }
}