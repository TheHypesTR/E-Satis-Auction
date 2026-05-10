using e_Sat_Auction.Interfaces.Repositories;
using e_Sat_Auction.Models.Common;

namespace e_Sat_Auction.Data.Repositories;

public class AddressRepository : GenericRepository<Address>, IAddressRepository
{
    public AddressRepository(AppDbContext context) : base(context)
    {
    }
}