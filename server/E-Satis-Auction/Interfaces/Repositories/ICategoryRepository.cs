using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Models.Categories;

namespace E_Satis_Auction.Interfaces.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
	Task<Category?> GetWithDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<Dictionary<Guid, string>> GetCategoryNamesByIdsAsync(IEnumerable<Guid> categoryIds, CancellationToken cancellationToken = default);
}