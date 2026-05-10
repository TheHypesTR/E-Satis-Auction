using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Models.Categories;

namespace e_Sat_Auction.Interfaces.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
	Task<Category?> GetWithDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<Dictionary<Guid, string>> GetCategoryNamesByIdsAsync(IEnumerable<Guid> categoryIds, CancellationToken cancellationToken = default);
}