using e_Sat_Auction.Interfaces.Repositories;
using e_Sat_Auction.Models.Categories;
using Microsoft.EntityFrameworkCore;

namespace e_Sat_Auction.Data.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetWithDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(category => category.Attributes)
                .ThenInclude(attribute => attribute.Options)
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);
    }
    
    public async Task<Dictionary<Guid, string>> GetCategoryNamesByIdsAsync(IEnumerable<Guid> categoryIds, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => categoryIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);
    }
}