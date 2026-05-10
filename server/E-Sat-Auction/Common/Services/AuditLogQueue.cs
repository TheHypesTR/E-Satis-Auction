using System.Threading.Channels;
using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Models.Common;

namespace e_Sat_Auction.Common.Services;

public class AuditLogQueue : IAuditLogQueue
{
    private readonly Channel<AuditLog> _channel = Channel.CreateUnbounded<AuditLog>();

    public async ValueTask EnqueueAsync(AuditLog log, CancellationToken cancellationToken = default)
        => await _channel.Writer.WriteAsync(log, cancellationToken);

    public IAsyncEnumerable<AuditLog> DequeueAllAsync(CancellationToken cancellationToken)
        => _channel.Reader.ReadAllAsync(cancellationToken);
}