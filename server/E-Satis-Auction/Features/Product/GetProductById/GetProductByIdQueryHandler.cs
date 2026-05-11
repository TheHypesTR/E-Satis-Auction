using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Facility;
using E_Satis_Auction.Dtos.Product;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Product.GetProductById;

using Models.Products;
using Models.Categories;

public sealed class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductDetailDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IFacilityRepository _facilityRepository;

    public GetProductByIdQueryHandler(
        IProductRepository productRepository,
        IItemRepository itemRepository,
        ICategoryRepository categoryRepository,
        IFacilityRepository facilityRepository)
    {
        _productRepository = productRepository;
        _itemRepository = itemRepository;
        _categoryRepository = categoryRepository;
        _facilityRepository = facilityRepository;
    }

    public async Task<ProductDetailDto> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        Product? product = await _productRepository.GetByIdAsync(query.Id, enableTracking: false, cancellationToken);
        NotFoundException.ThrowIfNull(product, ErrorMessages.Product.EntityName, query.Id);
        
        Category? category = await _categoryRepository.GetByIdAsync(product!.CategoryId, enableTracking: false, cancellationToken);
        NotFoundException.ThrowIfNull(category, ErrorMessages.Category.EntityName, product.CategoryId);
        
        List<ProductStockDto> facilityStocks = await GetFacilityStocksAsync(query.Id, cancellationToken);

        return MapToDetailDto(product, category!.Name, facilityStocks);
    }
    
    private async Task<List<ProductStockDto>> GetFacilityStocksAsync(Guid productId, CancellationToken cancellationToken)
    {
        Dictionary<Guid, int> stockSummary = await _itemRepository.GetAvailableStockSummaryAsync(productId, cancellationToken);
        if (stockSummary.Count is 0)
        {
            return [];
        }

        Dictionary<Guid, FacilityStockLookupDto> facilities = await _facilityRepository
            .GetFacilityStockInfoByIdsAsync(stockSummary.Keys, cancellationToken);

        return stockSummary
            .Where(stock => facilities.ContainsKey(stock.Key))
            .Select(stock => new ProductStockDto(
                stock.Key, 
                facilities[stock.Key].Name,
                facilities[stock.Key].Address,
                stock.Value))
            .ToList();
    }

    private static ProductDetailDto MapToDetailDto(Product product, string categoryName, List<ProductStockDto> facilityStocks)
    {
        return new ProductDetailDto(
            product.Id,
            product.Sku,
            product.Barcode,
            product.Name,
            product.CategoryId,
            categoryName,
            product.UnitOfMeasure,
            product.BaseAttributes,
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt,
            product.Version,
            facilityStocks
        );
    }
}