using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Commerce;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Data.Repositories;

public sealed class PurchaseOrderRepository : GenericRepository<PurchaseOrder>, IPurchaseOrderRepository
{
    public PurchaseOrderRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PurchaseOrder?> GetByIdWithDetailsAsync(Guid id, bool enableTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<PurchaseOrder> query = _dbSet
            .Include(order => order.Lines)
            .ThenInclude(line => line.Allocations);

        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
    }

    public async Task<PurchaseOrder?> GetByIdempotencyKeyWithDetailsAsync(string idempotencyKey, bool enableTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<PurchaseOrder> query = _dbSet
            .Include(order => order.Lines)
            .ThenInclude(line => line.Allocations);

        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(order => order.IdempotencyKey == idempotencyKey, cancellationToken);
    }

    public async Task<bool> HasLineForProductListingAsync(Guid productListingId, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrderLines
            .AsNoTracking()
            .AnyAsync(line => line.ProductListingId == productListingId, cancellationToken);
    }
}
