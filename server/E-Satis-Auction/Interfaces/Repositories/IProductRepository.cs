using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Models.Products;

namespace E_Satis_Auction.Interfaces.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Dictionary<Guid, string>> GetProductNamesByIdsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default);
    Task<List<Guid>> GetProductIdsBySearchTermAsync(string searchTerm, CancellationToken cancellationToken = default);
}