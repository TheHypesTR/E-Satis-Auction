using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Products;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Data.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Dictionary<Guid, string>> GetProductNamesByIdsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default)
    {
        List<Guid> ids = productIds.Distinct().ToList();
        if (ids.Count is 0)
        {
            return [];
        }

        return await _dbSet
            .AsNoTracking()
            .Where(p => ids.Contains(p.Id))
            .Select(p => new { p.Id, p.Name })
            .ToDictionaryAsync(p => p.Id, p => p.Name, cancellationToken);
    }

    public async Task<List<Guid>> GetProductIdsBySearchTermAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p =>
                p.Name.ToLower().Contains(searchTerm) ||
                p.Sku.ToLower().Contains(searchTerm) ||
                (p.Barcode != null && p.Barcode.ToLower().Contains(searchTerm)))
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);
    }
}