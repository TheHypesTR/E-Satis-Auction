using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Models.Products;

namespace E_Satis_Auction.Interfaces.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Dictionary<Guid, string>> GetProductNamesByIdsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default);
    Task<List<Guid>> GetProductIdsBySearchTermAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<List<Guid>> GetProductIdsByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, ProductListingProductEnrichmentDto>> GetProductListingEnrichmentsByIdsAsync(
        IEnumerable<Guid> productIds,
        CancellationToken cancellationToken = default);
}
