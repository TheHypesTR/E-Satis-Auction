using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Commerce;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Data.Repositories;

public sealed class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCartRepository
{
    public ShoppingCartRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ShoppingCart?> GetActiveByUserIdAsync(string userId, bool enableTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<ShoppingCart> query = _dbSet.Where(cart => cart.UserId == userId && cart.Status == CartStatus.Active);
        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }
}
