using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Commerce;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Data.Repositories;

public sealed class ReturnRequestRepository : GenericRepository<ReturnRequest>, IReturnRequestRepository
{
    public ReturnRequestRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ReturnRequest?> GetByIdWithLinesAsync(Guid id, bool enableTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<ReturnRequest> query = _dbSet.Include(returnRequest => returnRequest.Lines);
        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(returnRequest => returnRequest.Id == id, cancellationToken);
    }
}
