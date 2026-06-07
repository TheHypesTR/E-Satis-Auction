using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Models.Commerce;

namespace E_Satis_Auction.Interfaces.Repositories;

public interface IPaymentAttemptRepository : IGenericRepository<PaymentAttempt>
{
    Task<PaymentAttempt?> GetByIdempotencyKeyAsync(string idempotencyKey, bool enableTracking = false, CancellationToken cancellationToken = default);
    Task<PaymentAttempt?> GetByIdForUserAsync(Guid id, string userId, bool enableTracking = false, CancellationToken cancellationToken = default);
    Task<List<PaymentAttempt>> GetExpiredActiveAttemptsAsync(DateTimeOffset now, int take, CancellationToken cancellationToken = default);
}
