using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Categories;

namespace E_Satis_Auction.Data.Repositories;

public class CategoryAttributeOptionRepository : GenericRepository<CategoryAttributeOption>, ICategoryAttributeOptionRepository
{
    public CategoryAttributeOptionRepository(AppDbContext context) : base(context)
    {
    }
}