using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Models.Categories;

namespace e_Sat_Auction.Interfaces.Repositories;

public interface ICategoryAttributeRepository : IGenericRepository<CategoryAttribute>
{
    Task<CategoryAttribute?> GetWithOptionsForUpdateByIdAsync(Guid id, CancellationToken cancellationToken = default);
}