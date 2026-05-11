using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Common.Events;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Models.Items;

public sealed class Item : BaseEntity
{
    public Guid? ProductId { get; private set; }
    public ItemMode Mode { get; private set; }
    public Guid FacilityId { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid? SourceItemId { get; private set; }
    public string Name { get; private set; }
    public int Quantity { get; private set; }
    public UnitOfMeasure UnitOfMeasure { get; private set; }
    public ItemStatus Status { get; private set; }
    public uint Version { get; private set; }
    
    private Dictionary<string, string> _dynamicAttributes = [];
    public IReadOnlyDictionary<string, string> DynamicAttributes => _dynamicAttributes;

    private Item()
    {
        Name = string.Empty;
        Mode = ItemMode.AdHoc;
        UnitOfMeasure = UnitOfMeasure.Piece;
        Status = ItemStatus.Available;
    }

    public static Item CreateFromProduct(
        Guid productId,
        Guid categoryId,
        Guid facilityId,
        int quantity,
        UnitOfMeasure unitOfMeasure,
        ItemStatus status,
        Dictionary<string, string>? batchAttributes = null,
        InventoryTransactionType transactionType = InventoryTransactionType.StandardizedCreated,
        Guid? referenceId = null)
    {
        BusinessException.ThrowIfTrue(
            productId == Guid.Empty,
            ErrorMessages.Item.ProductIdRequiredForStandardized,
            ErrorMessages.Exception.InventoryTitle);
        
        Item item = CreateCore(
            ItemMode.Standardized,
            productId,
            categoryId,
            facilityId,
            string.Empty,
            quantity,
            unitOfMeasure,
            status);

        item.UpdateDynamicAttributes(batchAttributes ?? []);
        item.AddQuantityChangedEvent(transactionType, 0, quantity, referenceId);
        return item;
    }

    public static Item CreateAdHoc(
        Guid categoryId,
        Guid facilityId,
        string name,
        int quantity,
        UnitOfMeasure unitOfMeasure,
        ItemStatus status,
        Dictionary<string, string>? dynamicAttributes = null,
        InventoryTransactionType transactionType = InventoryTransactionType.AdHocCreated,
        Guid? referenceId = null)
    {
        BusinessException.ThrowIfNullOrWhiteSpace(
            name,
            ErrorMessages.Item.NameRequiredForAdHoc,
            ErrorMessages.Exception.InventoryTitle);

        Item item = CreateCore(
            ItemMode.AdHoc,
            null,
            categoryId,
            facilityId,
            name,
            quantity,
            unitOfMeasure,
            status);

        item.UpdateDynamicAttributes(dynamicAttributes ?? []);
        item.AddQuantityChangedEvent(transactionType, 0, quantity, referenceId);
        return item;
    }

    private static Item CreateCore(
        ItemMode mode,
        Guid? productId,
        Guid categoryId,
        Guid facilityId,
        string name,
        int quantity,
        UnitOfMeasure unitOfMeasure,
        ItemStatus status)
    {
        BusinessException.ThrowIfTrue(
            facilityId == Guid.Empty,
            ErrorMessages.Item.FacilityRequired,
            ErrorMessages.Exception.InventoryTitle);
        
        BusinessException.ThrowIfTrue(
            categoryId == Guid.Empty,
            ErrorMessages.Item.CategoryRequired,
            ErrorMessages.Exception.InventoryTitle);

        BusinessException.ThrowIfTrue(
            quantity < 0,
            ErrorMessages.Item.QuantityCannotBeNegative,
            ErrorMessages.Exception.InventoryTitle);

        ValidateModeConsistency(mode, productId, name);

        return new Item
        {
            ProductId = productId,
            Mode = mode,
            FacilityId = facilityId,
            CategoryId = categoryId,
            Name = name.Trim(),
            Quantity = quantity,
            UnitOfMeasure = unitOfMeasure,
            Status = status
        };
    }

    private static void ValidateModeConsistency(ItemMode mode, Guid? productId, string name)
    {
        switch (mode)
        {
            case ItemMode.Standardized:
                BusinessException.ThrowIfTrue(
                    productId is null || productId == Guid.Empty,
                    ErrorMessages.Item.ProductIdRequiredForStandardized,
                    ErrorMessages.Exception.InventoryTitle);
                
                break;

            case ItemMode.AdHoc:
                BusinessException.ThrowIfTrue(
                    productId is not null,
                    ErrorMessages.Item.ProductIdMustBeNullForAdHoc,
                    ErrorMessages.Exception.InventoryTitle);
                
                BusinessException.ThrowIfNullOrWhiteSpace(
                    name,
                    ErrorMessages.Item.NameRequiredForAdHoc,
                    ErrorMessages.Exception.InventoryTitle);
                
                break;
        }
    }

    private void UpdateDynamicAttributes(Dictionary<string, string> newAttributes)
    {
        _dynamicAttributes.Clear();

        foreach ((string key, string value) in newAttributes)
        {
            BusinessException.ThrowIfNullOrWhiteSpace(
                key,
                ErrorMessages.Item.DynamicAttributeKeyRequired,
                ErrorMessages.Exception.InventoryTitle);
            
            BusinessException.ThrowIfNullOrWhiteSpace(
                value,
                ErrorMessages.Item.DynamicAttributeValueRequired,
                ErrorMessages.Exception.InventoryTitle);

            _dynamicAttributes[key.ToSemanticCode()] = value.Trim();
        }
    }
    
    public void IncreaseQuantity(
        int amount,
        InventoryTransactionType transactionType = InventoryTransactionType.Adjusted,
        Guid? referenceId = null)
    {
        BusinessException.ThrowIfTrue(
            amount <= 0,
            ErrorMessages.Item.IncreaseAmountMustBePositive,
            ErrorMessages.Exception.InventoryTitle);

        int previousQuantity = Quantity;
        Quantity += amount;
        AddQuantityChangedEvent(transactionType, previousQuantity, Quantity, referenceId);
    }

    public void DecreaseQuantity(
        int amount,
        InventoryTransactionType transactionType = InventoryTransactionType.Adjusted,
        Guid? referenceId = null)
    {
        BusinessException.ThrowIfTrue(
            amount <= 0,
            ErrorMessages.Item.DecreaseAmountMustBePositive,
            ErrorMessages.Exception.InventoryTitle);

        BusinessException.ThrowIfTrue(
            Quantity - amount < 0,
            ErrorMessages.Dispatch.InsufficientStock,
            ErrorMessages.Exception.InventoryTitle);

        int previousQuantity = Quantity;
        Quantity -= amount;
        AddQuantityChangedEvent(transactionType, previousQuantity, Quantity, referenceId);
    }

    public void UpdateStatus(ItemStatus status)
    {
        Status = status;
    }

    public void Archive(
        InventoryTransactionType transactionType = InventoryTransactionType.Archived,
        Guid? referenceId = null)
    {
        int previousQuantity = Quantity;
        Quantity = 0;
        Status = ItemStatus.Archived;
        AddQuantityChangedEvent(transactionType, previousQuantity, Quantity, referenceId);
    }
    
    private void AddQuantityChangedEvent(
        InventoryTransactionType transactionType,
        int previousQuantity,
        int newQuantity,
        Guid? referenceId)
    {
        int quantityChange = newQuantity - previousQuantity;
        AddDomainEvent(new ItemQuantityChangedEvent(
            Id,
            FacilityId,
            transactionType,
            quantityChange,
            previousQuantity,
            newQuantity,
            referenceId));
    }
}