using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.InventoryTransactions;

namespace E_Satis_Auction.Data.Repositories;

public sealed class InventoryTransactionRepository : GenericRepository<InventoryTransaction>, IInventoryTransactionRepository
{
    public InventoryTransactionRepository(AppDbContext context) : base(context)
    {
    }
}