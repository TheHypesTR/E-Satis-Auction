using E_Satis_Auction.Common.Extensions;
using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Models.Categories;

namespace E_Satis_Auction.Features.Item.AddStandardizedItem;

using Models.Facilities;

public class AddStandardizedItemCommandHandler : ICommandHandler<AddStandardizedItemCommand, Guid>
{
    private readonly IItemRepository _itemRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public AddStandardizedItemCommandHandler(
        IItemRepository itemRepository,
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IFacilityRepository facilityRepository,
        ICurrentUserService currentUserService,
        ICacheService cacheService,
        IUnitOfWork unitOfWork)
    {
        _itemRepository = itemRepository;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _facilityRepository = facilityRepository;
        _currentUserService = currentUserService;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddStandardizedItemCommand command, CancellationToken cancellationToken)
    {
        await ValidateFacilityAndAuthorizationAsync(command.FacilityId, cancellationToken);
        
        Models.Products.Product? product = await _productRepository.GetByIdAsync(command.ProductId, enableTracking: false, cancellationToken);
        NotFoundException.ThrowIfNull(product, ErrorMessages.Product.EntityName, command.ProductId);
        BusinessException.ThrowIfFalse(
            product!.IsActive,
            ErrorMessages.Product.ProductNotEligibleForInventory,
            ErrorMessages.Exception.InventoryTitle);

        Models.Categories.Category? category = await _categoryRepository.GetWithDetailsByIdAsync(product.CategoryId, cancellationToken);
        NotFoundException.ThrowIfNull(category, ErrorMessages.Category.EntityName, product.CategoryId);
        BusinessException.ThrowIfFalse(
            category!.IsActive,
            ErrorMessages.Category.MustBeActiveToAddProduct,
            ErrorMessages.Exception.InventoryTitle);

        ValidateItemLevelAttributesAgainstSchema(category.Attributes, command.DynamicAttributes);

        Models.Items.Item item = Models.Items.Item.CreateFromProduct(
            command.ProductId,
            product.CategoryId,
            command.FacilityId,
            command.Quantity,
            command.UnitOfMeasure,
            command.Status,
            command.DynamicAttributes);

        await _itemRepository.AddAsync(item, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);
        
        await _cacheService.RemoveAsync(CacheKeys.GetProductById(command.ProductId), cancellationToken);

        return item.Id;
    }
    
    private async Task ValidateFacilityAndAuthorizationAsync(Guid facilityId, CancellationToken cancellationToken)
    {
        Facility? facility = await _facilityRepository.GetByIdAsync(facilityId, enableTracking: false, cancellationToken);
        NotFoundException.ThrowIfNull(facility, ErrorMessages.Facility.EntityName, facilityId);

        BusinessException.ThrowIfTrue(
            facility!.Status is not ApprovalStatus.Approved,
            ErrorMessages.Facility.MustBeApproved,
            ErrorMessages.Exception.InventoryTitle);

        bool hasAccess = await _currentUserService.HasFacilityAccess(facilityId, cancellationToken);
        ForbiddenAccessException.ThrowIfFalse(
            hasAccess, 
            ErrorMessages.Auth.UnauthorizedAccess,
            ErrorMessages.Exception.UnauthorizedAccess);
    }

    private static void ValidateItemLevelAttributesAgainstSchema(
        IReadOnlyCollection<CategoryAttribute> schemaAttributes,
        Dictionary<string, string>? requestAttributes)
    {
        requestAttributes ??= [];

        Dictionary<string, string> normalizedRequest = [];
        foreach (KeyValuePair<string, string> kvp in requestAttributes)
        {
            string semanticKey = kvp.Key.ToSemanticCode();
            BusinessException.ThrowIfTrue(
                normalizedRequest.ContainsKey(semanticKey),
                ErrorMessages.Item.DuplicateDynamicAttributeKey,
                ErrorMessages.Exception.InventoryTitle);
        
            normalizedRequest[semanticKey] = kvp.Value.Trim();
        }

        List<CategoryAttribute> itemLevelAttributes = schemaAttributes
            .Where(a => a.Target == AttributeTarget.ItemLevel)
            .ToList();

        IEnumerable<string> requiredKeys = itemLevelAttributes
            .Where(a => a.IsRequired)
            .Select(a => a.Code);

        foreach (string requiredKey in requiredKeys)
        {
            BusinessException.ThrowIfFalse(
                normalizedRequest.ContainsKey(requiredKey),
                ErrorMessages.Item.RequiredAttributeMissing,
                ErrorMessages.Exception.InventoryTitle);
        }

        foreach ((string key, string value) in normalizedRequest)
        {
            CategoryAttribute? attribute = itemLevelAttributes.FirstOrDefault(a => a.Code == key);
            BusinessException.ThrowIfFalse(
                attribute is not null,
                ErrorMessages.Item.InvalidAttributeKey,
                ErrorMessages.Exception.InventoryTitle);

            if (attribute!.DataType is AttributeDataType.SelectList)
            {
                bool isValidOption = attribute.Options
                    .Any(o => o.Value.Equals(value, StringComparison.OrdinalIgnoreCase));

                BusinessException.ThrowIfFalse(
                    isValidOption,
                    ErrorMessages.Item.InvalidAttributeValue,
                    ErrorMessages.Exception.InventoryTitle);
            }
        }
    }
}