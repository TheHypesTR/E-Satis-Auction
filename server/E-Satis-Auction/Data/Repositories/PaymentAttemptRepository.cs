using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Commerce;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Data.Repositories;

public sealed class PaymentAttemptRepository : GenericRepository<PaymentAttempt>, IPaymentAttemptRepository
{
    public PaymentAttemptRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PaymentAttempt?> GetByIdempotencyKeyAsync(string idempotencyKey, bool enableTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<PaymentAttempt> query = _dbSet.Where(payment => payment.IdempotencyKey == idempotencyKey);
        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PaymentAttempt?> GetByIdForUserAsync(Guid id, string userId, bool enableTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<PaymentAttempt> query = _dbSet.Where(payment => payment.Id == id && payment.UserId == userId);
        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<PaymentAttempt>> GetExpiredActiveAttemptsAsync(DateTimeOffset now, int take, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(payment =>
                (payment.Status == PaymentStatus.PaymentEntry || payment.Status == PaymentStatus.Processing) &&
                payment.ExpiresAt <= now)
            .OrderBy(payment => payment.ExpiresAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
