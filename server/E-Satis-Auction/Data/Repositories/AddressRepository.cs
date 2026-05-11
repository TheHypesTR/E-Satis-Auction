using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Common;

namespace E_Satis_Auction.Data.Repositories;

public class AddressRepository : GenericRepository<Address>, IAddressRepository
{
    public AddressRepository(AppDbContext context) : base(context)
    {
    }
}