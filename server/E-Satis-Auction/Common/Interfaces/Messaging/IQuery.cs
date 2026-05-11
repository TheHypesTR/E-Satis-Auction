using MediatR;

namespace E_Satis_Auction.Common.Interfaces.Messaging;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}

public interface ICacheableQueryMarker
{
    string CacheKey { get; }
    TimeSpan? Expiration { get; }
    bool BypassCache { get; }
}

public interface ICacheableQuery<out TResponse> : IQuery<TResponse>, ICacheableQueryMarker
{
}

