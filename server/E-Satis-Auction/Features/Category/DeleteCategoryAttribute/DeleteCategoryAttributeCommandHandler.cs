using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.Category.DeleteCategoryAttribute;

using Models.Categories;

public sealed class DeleteCategoryAttributeCommandHandler : ICommandHandler<DeleteCategoryAttributeCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICategoryAttributeRepository _categoryAttributeRepository;
    private readonly IProductRepository _productRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryAttributeCommandHandler(
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

    public async Task Handle(DeleteCategoryAttributeCommand command, CancellationToken cancellationToken)
    {
        Category? category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(category, ErrorMessages.Category.EntityName, command.CategoryId);

        await EnsureCategorySchemaCanBeMutated(category!, cancellationToken);

        CategoryAttribute? attribute = await _categoryAttributeRepository.GetWithOptionsForUpdateByIdAsync(command.AttributeId, cancellationToken);
        NotFoundException.ThrowIfNull(attribute, ErrorMessages.Category.AttributeEntityName, command.AttributeId);
        NotFoundException.ThrowIfNull(
            attribute!.CategoryId == command.CategoryId ? attribute : null,
            ErrorMessages.Category.AttributeEntityName,
            command.AttributeId);

        attribute.DeleteWithOptions();

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