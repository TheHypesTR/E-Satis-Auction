using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Models.Products;

public sealed class Product : BaseEntity
{
    public string Sku { get; private set; }
    public string? Barcode { get; private set; }
    public string Name { get; private set; }
    public Guid CategoryId { get; private set; }
    public UnitOfMeasure UnitOfMeasure { get; private set; }
    public bool IsActive { get; private set; }
    public uint Version { get; private set; }

    private Dictionary<string, string> _baseAttributes = [];
    public IReadOnlyDictionary<string, string> BaseAttributes => _baseAttributes;

    private Product()
    {
        Sku = string.Empty;
        Name = string.Empty;
        UnitOfMeasure = UnitOfMeasure.Piece;
        IsActive = true;
    }

    public static Product Create(
        string sku,
        string? barcode,
        string name,
        Guid categoryId,
        UnitOfMeasure unitOfMeasure,
        Dictionary<string, string>? baseAttributes = null)
    {
        BusinessException.ThrowIfNullOrWhiteSpace(
            sku,
            ErrorMessages.Product.SkuRequired,
            ErrorMessages.Exception.ProductTitle);
        
        BusinessException.ThrowIfNullOrWhiteSpace(
            name,
            ErrorMessages.Product.NameRequired,
            ErrorMessages.Exception.ProductTitle);
        
        BusinessException.ThrowIfTrue(
            categoryId == Guid.Empty,
            ErrorMessages.Product.CategoryRequired,
            ErrorMessages.Exception.ProductTitle);

        Product product = new()
        {
            Sku = sku.Trim().ToUpperInvariant(),
            Barcode = string.IsNullOrWhiteSpace(barcode) ? null : barcode.Trim(),
            Name = name.Trim(),
            CategoryId = categoryId,
            UnitOfMeasure = unitOfMeasure,
            IsActive = true
        };

        product.UpdateBaseAttributes(baseAttributes ?? []);
        return product;
    }

    public void UpdateBaseAttributes(Dictionary<string, string> newAttributes)
    {
        BusinessException.ThrowIfFalse(
            IsActive,
            ErrorMessages.Product.InactiveProductUpdateNotAllowed,
            ErrorMessages.Exception.ProductTitle);

        _baseAttributes.Clear();
        foreach ((string key, string value) in newAttributes)
        {
            BusinessException.ThrowIfNullOrWhiteSpace(
                key,
                ErrorMessages.Product.AttributeKeyRequired,
                ErrorMessages.Exception.ProductTitle);
            
            BusinessException.ThrowIfNullOrWhiteSpace(
                value,
                ErrorMessages.Product.AttributeValueRequired,
                ErrorMessages.Exception.ProductTitle);

            _baseAttributes[key.ToSemanticCode()] = value.Trim();
        }
    }
    
    public void Activate()
    {
        BusinessException.ThrowIfTrue(
            IsActive,
            ErrorMessages.Product.AlreadyActive,
            ErrorMessages.Exception.ProductTitle);
        
        IsActive = true;
    }

    public void Deactivate()
    {
        BusinessException.ThrowIfTrue(
            !IsActive,
            ErrorMessages.Product.AlreadyInactive,
            ErrorMessages.Exception.ProductTitle);
        
        IsActive = false;
    }
}