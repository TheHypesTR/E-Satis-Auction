using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Data;
using E_Satis_Auction.Models.Common;

namespace E_Satis_Auction.Common.Workers;

public class AuditLogWorker : BackgroundService
{
    private readonly IAuditLogQueue _queue;
    private readonly IServiceProvider _serviceProvider;

    private readonly TimeSpan _flushInterval = TimeSpan.FromSeconds(15);
    private const int BATCH_SIZE = 100;

    public AuditLogWorker(IAuditLogQueue queue, IServiceProvider serviceProvider)
    {
        _queue = queue;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            List<AuditLog> logs = [];
            using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            cts.CancelAfter(_flushInterval);

            try
            {
                await foreach (AuditLog log in _queue.DequeueAllAsync(cts.Token))
                {
                    logs.Add(log);
                    if (logs.Count >= BATCH_SIZE) break;
                }
            }
            catch (OperationCanceledException)
            {
            }

            if (logs.Count is not 0)
            {
                using IServiceScope scope = _serviceProvider.CreateScope();
                AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await context.AuditLogs.AddRangeAsync(logs, stoppingToken);
                await context.SaveChangesAsync(stoppingToken);
            }
        }
    }
}