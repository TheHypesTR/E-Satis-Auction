using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Exceptions;
using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Interfaces;
using e_Sat_Auction.Interfaces.Repositories;
using e_Sat_Auction.Common.Extensions;

namespace e_Sat_Auction.Features.Category.UpdateCategoryAttribute;

using Models.Categories;

public sealed class UpdateCategoryAttributeCommandHandler : ICommandHandler<UpdateCategoryAttributeCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICategoryAttributeRepository _categoryAttributeRepository;
    private readonly IProductRepository _productRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryAttributeCommandHandler(
        ICategoryRepository categoryRepository,
        ICategoryAttributeRepository categoryAttributeRepository,
        IProductRepository productRepository,
        IItemRepository itemRepository,
        ICacheService cacheService,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _categoryAttributeRepository = categoryAttributeRepository;
        _productRepository = productRepository;
        _itemRepository = itemRepository;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateCategoryAttributeCommand command, CancellationToken cancellationToken)
    {
        Category? category = await _categoryRepository.GetByIdAsync(command.CategoryId, enableTracking: false, cancellationToken);
        NotFoundException.ThrowIfNull(category, ErrorMessages.Category.EntityName, command.CategoryId);

        await EnsureCategorySchemaCanBeMutated(category!, cancellationToken);

        string newCode = command.Code.ToSemanticCode();
        bool hasDuplicateCode = await _categoryAttributeRepository
            .AnyAsync(a => a.CategoryId == command.CategoryId && a.Id != command.AttributeId && a.Code == newCode, cancellationToken);
        
        BusinessException.ThrowIfTrue(
            hasDuplicateCode,
            ErrorMessages.Category.DuplicateAttributeCode,
            ErrorMessages.Exception.CategoryTitle);

        CategoryAttribute? trackedAttribute = await _categoryAttributeRepository.GetWithOptionsForUpdateByIdAsync(command.AttributeId, cancellationToken);
        NotFoundException.ThrowIfNull(trackedAttribute, ErrorMessages.Category.AttributeEntityName, command.AttributeId);
        NotFoundException.ThrowIfNull(
            trackedAttribute!.CategoryId == command.CategoryId ? trackedAttribute : null,
            ErrorMessages.Category.AttributeEntityName,
            command.AttributeId);

        trackedAttribute.UpdateDetails(command.Name, command.Code, command.DataType, command.Target, command.IsRequired);
        
        await _unitOfWork.CompleteAsync(cancellationToken);
        await _cacheService.RemoveAsync(CacheKeys.GetCategoryById(command.CategoryId), cancellationToken);
    }

    private async Task EnsureCategorySchemaCanBeMutated(Category category, CancellationToken cancellationToken)
    {
        bool hasProducts = await _productRepository.AnyAsync(product => product.CategoryId == category.Id, cancellationToken);
        bool hasItems = await _itemRepository.AnyAsync(item => item.CategoryId == category.Id, cancellationToken);

        category.EnsureCanMutateSchema(hasProducts || hasItems);
    }
}