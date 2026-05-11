using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Models.Categories;

namespace E_Satis_Auction.Interfaces.Repositories;

public interface ICategoryAttributeRepository : IGenericRepository<CategoryAttribute>
{
    Task<CategoryAttribute?> GetWithOptionsForUpdateByIdAsync(Guid id, CancellationToken cancellationToken = default);
}