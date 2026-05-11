using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Common.Models;
using E_Satis_Auction.Dtos.Product;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Product.GetAllProducts;

using Models.Products;

public class GetAllProductsQueryHandler : IQueryHandler<GetAllProductsQuery, PaginatedList<ProductSummaryDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public GetAllProductsQueryHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<PaginatedList<ProductSummaryDto>> Handle(GetAllProductsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Product> productQuery = _productRepository.GetAllAsQueryable();
        productQuery = ApplyFilters(productQuery, query);
        
        PaginatedList<Product> paginatedProducts = await productQuery
            .OrderByDescending(p => p.UpdatedAt)
            .ToPaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);
        
        if (paginatedProducts.Items.Count is 0)
        {
            return new PaginatedList<ProductSummaryDto>([], 0, query.PageNumber, query.PageSize);
        }
        
        Dictionary<Guid, string> categoryNames = await GetCategoryNamesAsync(paginatedProducts.Items, cancellationToken);

        return MapToPaginatedDto(paginatedProducts, categoryNames, query.PageSize);
    }
    
    private static IQueryable<Product> ApplyFilters(IQueryable<Product> query, GetAllProductsQuery filters)
    {
        if (filters.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == filters.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
        {
            string searchTerm = filters.SearchTerm.Trim().ToLower();

            query = query.Where(p =>
                (p.Barcode != null && p.Barcode == filters.SearchTerm.Trim()) || 
                p.Sku.ToLower() == searchTerm || 
                p.Name.ToLower().Contains(searchTerm));
        }

        return query;
    }
    
    private async Task<Dictionary<Guid, string>> GetCategoryNamesAsync(
        IReadOnlyCollection<Product> products, 
        CancellationToken cancellationToken)
    {
        IEnumerable<Guid> categoryIds = products.Select(p => p.CategoryId).Distinct();
        return await _categoryRepository.GetCategoryNamesByIdsAsync(categoryIds, cancellationToken);
    }

    private static PaginatedList<ProductSummaryDto> MapToPaginatedDto(
        PaginatedList<Product> paginatedProducts,
        Dictionary<Guid, string> categoryNames,
        int pageSize)
    {
        List<ProductSummaryDto> dtoList = paginatedProducts.Items.Select(product => new ProductSummaryDto(
            product.Id,
            product.Sku,
            product.Barcode,
            product.Name,
            categoryNames[product.CategoryId],
            product.UnitOfMeasure,
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt,
            product.Version
        )).ToList();

        return new PaginatedList<ProductSummaryDto>(dtoList, paginatedProducts.TotalCount, paginatedProducts.PageNumber, pageSize);
    }
}