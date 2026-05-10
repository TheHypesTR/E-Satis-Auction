using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Exceptions;
using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Enums;
using e_Sat_Auction.Interfaces;
using e_Sat_Auction.Interfaces.Repositories;
using e_Sat_Auction.Models.Categories;
using e_Sat_Auction.Common.Extensions;

namespace e_Sat_Auction.Features.Item.AddAdHocItem;

using Models.Facilities;

public class AddAdHocItemCommandHandler : ICommandHandler<AddAdHocItemCommand, Guid>
{
    private readonly IItemRepository _itemRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AddAdHocItemCommandHandler(
        IItemRepository itemRepository,
        ICategoryRepository categoryRepository,
        IFacilityRepository facilityRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _itemRepository = itemRepository;
        _categoryRepository = categoryRepository;
        _facilityRepository = facilityRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddAdHocItemCommand command, CancellationToken cancellationToken)
    {
        await ValidateFacilityAndAuthorizationAsync(command.FacilityId, cancellationToken);
        
        Models.Categories.Category? category = await _categoryRepository.GetWithDetailsByIdAsync(command.CategoryId, cancellationToken);
        NotFoundException.ThrowIfNull(category, ErrorMessages.Category.EntityName, command.CategoryId);
        BusinessException.ThrowIfFalse(
            category!.IsActive,
            ErrorMessages.Category.MustBeActiveToAddProduct,
            ErrorMessages.Exception.InventoryTitle);

        ValidateItemLevelAttributesAgainstSchema(category.Attributes, command.DynamicAttributes);
        
        Models.Items.Item item = Models.Items.Item.CreateAdHoc(
            command.CategoryId,
            command.FacilityId,
            command.Name,
            command.Quantity,
            command.UnitOfMeasure,
            command.Status,
            command.DynamicAttributes);

        await _itemRepository.AddAsync(item, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

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