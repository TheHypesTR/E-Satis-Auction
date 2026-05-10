using e_Sat_Auction.Common.Options;

namespace e_Sat_Auction.Extensions;

public static class RedisCachingExtensions
{
    public static IServiceCollection AddRedisCaching(this IServiceCollection services, IConfiguration configuration)
    {
        RedisOptions? redisSettings = configuration.GetSection(RedisOptions.SectionName).Get<RedisOptions>();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisSettings?.ConnectionString;
            options.InstanceName = redisSettings?.InstanceName;
        });

        return services;
    }
}