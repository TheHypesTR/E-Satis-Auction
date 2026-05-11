using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Category.AddCategoryAttributeOption;

using Models.Categories;

public sealed class AddCategoryAttributeOptionCommandHandler : ICommandHandler<AddCategoryAttributeOptionCommand, Guid>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICategoryAttributeRepository _categoryAttributeRepository;
    private readonly ICategoryAttributeOptionRepository _categoryAttributeOptionRepository;
    private readonly IProductRepository _productRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public AddCategoryAttributeOptionCommandHandler(
        ICategoryRepository categoryRepository,
        ICategoryAttributeRepository categoryAttributeRepository,
        ICategoryAttributeOptionRepository categoryAttributeOptionRepository,
        IProductRepository productRepository,
        IItemRepository itemRepository,
        ICacheService cacheService,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _categoryAttributeRepository = categoryAttributeRepository;
        _categoryAttributeOptionRepository = categoryAttributeOptionRepository;
        _productRepository = productRepository;
        _itemRepository = itemRepository;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddCategoryAttributeOptionCommand command, CancellationToken cancellationToken)
    {
        Category? category = await _categoryRepository.GetByIdAsync(command.CategoryId, enableTracking: false, cancellationToken);
        NotFoundException.ThrowIfNull(category, ErrorMessages.Category.EntityName, command.CategoryId);

        await EnsureCategorySchemaCanBeMutated(category!, cancellationToken);
        
        CategoryAttribute? attribute = await _categoryAttributeRepository.GetByIdAsync(command.AttributeId, enableTracking: false, cancellationToken);
        NotFoundException.ThrowIfNull(attribute, ErrorMessages.Category.AttributeEntityName, command.AttributeId);
        NotFoundException.ThrowIfNull(
            attribute!.CategoryId == command.CategoryId ? attribute : null,
            ErrorMessages.Category.AttributeEntityName,
            command.AttributeId);
        
        BusinessException.ThrowIfFalse(
            attribute.DataType is AttributeDataType.SelectList,
            ErrorMessages.Category.OptionOnlyForSelectList,
            ErrorMessages.Exception.CategoryTitle);
        
        string normalizedValue = command.Value.Trim();
        bool hasDuplicateOption = await _categoryAttributeOptionRepository
            .AnyAsync(o => o.CategoryAttributeId == command.AttributeId && o.Value.ToLower() == normalizedValue.ToLower(), cancellationToken);

        BusinessException.ThrowIfTrue(
            hasDuplicateOption,
            ErrorMessages.Category.DuplicateOptionValue,
            ErrorMessages.Exception.CategoryTitle);
        
        CategoryAttributeOption newOption = CategoryAttributeOption.Create(command.AttributeId, normalizedValue);

        await _categoryAttributeOptionRepository.AddAsync(newOption, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);
        await _cacheService.RemoveAsync(CacheKeys.GetCategoryById(command.CategoryId), cancellationToken);
        
        return newOption.Id;
    }

    private async Task EnsureCategorySchemaCanBeMutated(Category category, CancellationToken cancellationToken)
    {
        bool hasProducts = await _productRepository.AnyAsync(product => product.CategoryId == category.Id, cancellationToken);
        bool hasItems = await _itemRepository.AnyAsync(item => item.CategoryId == category.Id, cancellationToken);

        category.EnsureCanMutateSchema(hasProducts || hasItems);
    }
}