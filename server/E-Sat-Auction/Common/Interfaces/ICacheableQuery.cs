namespace e_Sat_Auction.Common.Interfaces;

public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan? Expiration { get; }
    bool BypassCache { get; }
}