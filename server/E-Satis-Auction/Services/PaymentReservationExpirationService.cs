using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Interfaces.Services;

namespace E_Satis_Auction.Services;

public sealed class PaymentReservationExpirationService : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMinutes(1);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PaymentReservationExpirationService> _logger;

    public PaymentReservationExpirationService(IServiceScopeFactory scopeFactory, ILogger<PaymentReservationExpirationService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(PollInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await ExpireReservationsAsync(stoppingToken);
        }
    }

    private async Task ExpireReservationsAsync(CancellationToken cancellationToken)
    {
        try
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            IPaymentAttemptRepository paymentAttemptRepository = scope.ServiceProvider.GetRequiredService<IPaymentAttemptRepository>();
            ICommerceWorkflowService commerceWorkflowService = scope.ServiceProvider.GetRequiredService<ICommerceWorkflowService>();

            List<Models.Commerce.PaymentAttempt> expiredAttempts = await paymentAttemptRepository.GetExpiredActiveAttemptsAsync(DateTimeOffset.UtcNow, 50, cancellationToken);
            foreach (Models.Commerce.PaymentAttempt attempt in expiredAttempts)
            {
                await commerceWorkflowService.ExpirePaymentAsync(attempt, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Payment reservation expiration sweep failed.");
        }
    }
}
