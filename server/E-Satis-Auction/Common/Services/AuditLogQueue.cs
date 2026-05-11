using System.Threading.Channels;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Models.Common;

namespace E_Satis_Auction.Common.Services;

public class AuditLogQueue : IAuditLogQueue
{
    private readonly Channel<AuditLog> _channel = Channel.CreateUnbounded<AuditLog>();

    public async ValueTask EnqueueAsync(AuditLog log, CancellationToken cancellationToken = default)
        => await _channel.Writer.WriteAsync(log, cancellationToken);

    public IAsyncEnumerable<AuditLog> DequeueAllAsync(CancellationToken cancellationToken)
        => _channel.Reader.ReadAllAsync(cancellationToken);
}