using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Commerce;

namespace E_Satis_Auction.Data.Repositories;

public sealed class UserSaleRequestRepository : GenericRepository<UserSaleRequest>, IUserSaleRequestRepository
{
    public UserSaleRequestRepository(AppDbContext context) : base(context)
    {
    }
}
