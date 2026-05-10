using e_Sat_Auction.Models.Common;

namespace e_Sat_Auction.Common.Interfaces;

public interface IAuditLogQueue
{
    ValueTask EnqueueAsync(AuditLog log, CancellationToken cancellationToken = default);
    IAsyncEnumerable<AuditLog> DequeueAllAsync(CancellationToken cancellationToken);
}