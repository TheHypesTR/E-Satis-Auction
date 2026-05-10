using e_Sat_Auction.Interfaces.Repositories;
using e_Sat_Auction.Models.Categories;
using Microsoft.EntityFrameworkCore;

namespace e_Sat_Auction.Data.Repositories;

public sealed class CategoryAttributeRepository : GenericRepository<CategoryAttribute>, ICategoryAttributeRepository
{
    public CategoryAttributeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<CategoryAttribute?> GetWithOptionsForUpdateByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(attribute => attribute.Options)
            .FirstOrDefaultAsync(attribute => attribute.Id == id, cancellationToken);
    }
}