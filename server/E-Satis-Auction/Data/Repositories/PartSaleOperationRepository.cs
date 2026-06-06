using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Commerce;

namespace E_Satis_Auction.Data.Repositories;

public sealed class PartSaleOperationRepository : GenericRepository<PartSaleOperation>, IPartSaleOperationRepository
{
    public PartSaleOperationRepository(AppDbContext context) : base(context)
    {
    }
}
