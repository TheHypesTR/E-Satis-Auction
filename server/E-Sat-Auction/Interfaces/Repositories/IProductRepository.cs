using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Models.Products;

namespace e_Sat_Auction.Interfaces.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Dictionary<Guid, string>> GetProductNamesByIdsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default);
    Task<List<Guid>> GetProductIdsBySearchTermAsync(string searchTerm, CancellationToken cancellationToken = default);
}