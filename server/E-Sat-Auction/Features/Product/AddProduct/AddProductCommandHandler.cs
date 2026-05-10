using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Exceptions;
using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Enums;
using e_Sat_Auction.Interfaces.Repositories;
using e_Sat_Auction.Common.Extensions;

namespace e_Sat_Auction.Features.Product.AddProduct;

using Models.Products;
using Models.Categories;

public class AddProductCommandHandler : ICommandHandler<AddProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddProductCommand command, CancellationToken cancellationToken)
    {
        string normalizedSku = command.Sku.Trim().ToLowerInvariant();
        bool skuExists = await _productRepository.AnyAsync(p => p.Sku.ToLower() == normalizedSku, cancellationToken);
        
        BusinessException.ThrowIfTrue(
            skuExists,
            ErrorMessages.Product.SkuAlreadyExists,
            ErrorMessages.Exception.ProductTitle);
        
        Category? category = await _categoryRepository.GetWithDetailsByIdAsync(command.CategoryId, cancellationToken);
        NotFoundException.ThrowIfNull(category, ErrorMessages.Category.EntityName, command.CategoryId);

        BusinessException.ThrowIfFalse(
            category!.IsActive,
            ErrorMessages.Category.MustBeActiveToAddProduct,
            ErrorMessages.Exception.ProductTitle);
        
        ValidateAttributesAgainstSchema(category.Attributes, command.BaseAttributes);
        
        Product product = Product.Create(
            command.Sku,
            command.Barcode,
            command.Name,
            command.CategoryId,
            command.UnitOfMeasure,
            command.BaseAttributes);

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return product.Id;
    }
    
    private static void ValidateAttributesAgainstSchema(
        IReadOnlyCollection<CategoryAttribute> schemaAttributes,
        Dictionary<string, string>? requestAttributes)
    {
        requestAttributes ??= [];
        
        Dictionary<string, string> normalizedRequest = requestAttributes
            .ToDictionary(kvp => kvp.Key.ToSemanticCode(), kvp => kvp.Value.Trim());

        List<CategoryAttribute> productLevelAttributes = schemaAttributes
            .Where(a => a.Target == AttributeTarget.ProductLevel)
            .ToList();
        
        IEnumerable<string> requiredKeys = productLevelAttributes
            .Where(a => a.IsRequired)
            .Select(a => a.Code);
        
        foreach (string requiredKey in requiredKeys)
        {
            BusinessException.ThrowIfFalse(
                normalizedRequest.ContainsKey(requiredKey),
                ErrorMessages.Product.RequiredAttributeMissing,
                ErrorMessages.Exception.ProductTitle);
        }
        
        foreach ((string key, string value) in normalizedRequest)
        {
            CategoryAttribute? attribute = productLevelAttributes.FirstOrDefault(a => a.Code == key);
            BusinessException.ThrowIfFalse(
                attribute is not null,
                ErrorMessages.Product.InvalidAttributeKey,
                ErrorMessages.Exception.ProductTitle);

            if (attribute!.DataType is AttributeDataType.SelectList)
            {
                bool isValidOption = attribute.Options
                    .Any(o => o.Value.Equals(value, StringComparison.OrdinalIgnoreCase));

                BusinessException.ThrowIfFalse(
                    isValidOption,
                    ErrorMessages.Product.InvalidAttributeValue,
                    ErrorMessages.Exception.ProductTitle);
            }
        }
    }
}