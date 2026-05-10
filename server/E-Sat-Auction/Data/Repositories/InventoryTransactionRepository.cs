using e_Sat_Auction.Interfaces.Repositories;
using e_Sat_Auction.Models.InventoryTransactions;

namespace e_Sat_Auction.Data.Repositories;

public sealed class InventoryTransactionRepository : GenericRepository<InventoryTransaction>, IInventoryTransactionRepository
{
    public InventoryTransactionRepository(AppDbContext context) : base(context)
    {
    }
}