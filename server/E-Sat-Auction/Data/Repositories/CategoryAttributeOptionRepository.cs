using e_Sat_Auction.Interfaces.Repositories;
using e_Sat_Auction.Models.Categories;

namespace e_Sat_Auction.Data.Repositories;

public class CategoryAttributeOptionRepository : GenericRepository<CategoryAttributeOption>, ICategoryAttributeOptionRepository
{
    public CategoryAttributeOptionRepository(AppDbContext context) : base(context)
    {
    }
}