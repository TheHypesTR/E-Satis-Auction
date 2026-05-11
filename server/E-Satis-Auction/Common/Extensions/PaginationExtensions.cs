using E_Satis_Auction.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Common.Extensions;

public static class PaginationExtensions
{
    public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(
        this IQueryable<T> source, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        int count = await source.CountAsync(cancellationToken);

        List<T> items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}