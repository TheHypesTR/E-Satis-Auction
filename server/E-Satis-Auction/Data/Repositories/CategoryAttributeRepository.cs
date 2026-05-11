using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Categories;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Data.Repositories;

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