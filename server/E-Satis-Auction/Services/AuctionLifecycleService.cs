using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Interfaces.Services;

namespace E_Satis_Auction.Services;

public sealed class AuctionLifecycleService : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(30);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AuctionLifecycleService> _logger;

    public AuctionLifecycleService(IServiceScopeFactory scopeFactory, ILogger<AuctionLifecycleService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(PollInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await SweepAsync(stoppingToken);
        }
    }

    private async Task SweepAsync(CancellationToken cancellationToken)
    {
        try
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            IAuctionRepository auctionRepository = scope.ServiceProvider.GetRequiredService<IAuctionRepository>();
            IAuctionWorkflowService auctionWorkflowService = scope.ServiceProvider.GetRequiredService<IAuctionWorkflowService>();

            DateTimeOffset now = DateTimeOffset.UtcNow;

            List<Models.Commerce.Auction> scheduledAuctions = await auctionRepository.GetScheduledToActivateAsync(now, 25, cancellationToken);
            foreach (Models.Commerce.Auction auction in scheduledAuctions)
            {
                await auctionWorkflowService.ActivateAuctionAsync(auction.Id, cancellationToken);
            }

            List<Models.Commerce.Auction> endedAuctions = await auctionRepository.GetActiveToFinalizeAsync(now, 25, cancellationToken);
            foreach (Models.Commerce.Auction auction in endedAuctions)
            {
                await auctionWorkflowService.FinalizeAuctionAsync(auction.Id, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Auction lifecycle sweep failed.");
        }
    }
}
