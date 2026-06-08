using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Models.Commerce;

namespace E_Satis_Auction.Interfaces.Repositories;

public interface IShoppingCartRepository : IGenericRepository<ShoppingCart>
{
    Task<ShoppingCart?> GetActiveByUserIdAsync(string userId, bool enableTracking = false, CancellationToken cancellationToken = default);
}
