using E_Satis_Auction.Models.Common;

namespace E_Satis_Auction.Common.Interfaces;

public interface IAuditLogQueue
{
    ValueTask EnqueueAsync(AuditLog log, CancellationToken cancellationToken = default);
    IAsyncEnumerable<AuditLog> DequeueAllAsync(CancellationToken cancellationToken);
}