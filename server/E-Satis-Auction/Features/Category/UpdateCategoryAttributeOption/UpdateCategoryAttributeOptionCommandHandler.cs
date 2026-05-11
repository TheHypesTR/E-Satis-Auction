using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Category.UpdateCategoryAttributeOption;

using Models.Categories;

public sealed class UpdateCategoryAttributeOptionCommandHandler : ICommandHandler<UpdateCategoryAttributeOptionCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICategoryAttributeRepository _categoryAttributeRepository;
    private readonly IProductRepository _productRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryAttributeOptionCommandHandler(
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

    public async Task Handle(UpdateCategoryAttributeOptionCommand command, CancellationToken cancellationToken)
    {
        Category? category = await _categoryRepository.GetByIdAsync(command.CategoryId, enableTracking: false, cancellationToken);
        NotFoundException.ThrowIfNull(category, ErrorMessages.Category.EntityName, command.CategoryId);

        await EnsureCategorySchemaCanBeMutated(category!, cancellationToken);

        CategoryAttribute? trackedAttribute = await _categoryAttributeRepository.GetWithOptionsForUpdateByIdAsync(command.AttributeId, cancellationToken);
        NotFoundException.ThrowIfNull(trackedAttribute, ErrorMessages.Category.AttributeEntityName, command.AttributeId);
        NotFoundException.ThrowIfNull(
            trackedAttribute!.CategoryId == command.CategoryId ? trackedAttribute : null,
            ErrorMessages.Category.AttributeEntityName,
            command.AttributeId);

        trackedAttribute.UpdateOption(command.OptionId, command.Value);
        
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