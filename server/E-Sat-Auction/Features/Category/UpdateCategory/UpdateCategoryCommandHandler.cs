using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Exceptions;
using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Interfaces;
using e_Sat_Auction.Interfaces.Repositories;
using e_Sat_Auction.Common.Extensions;

namespace e_Sat_Auction.Features.Category.UpdateCategory;

using Models.Categories;

public sealed class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        ICacheService cacheService,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        Category? category = await _categoryRepository.GetByIdAsync(command.Id, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(category, ErrorMessages.Category.EntityName, command.Id);

        string normalizedName = command.Name.ToSemanticCode();
        bool existsCategory = await _categoryRepository
            .AnyAsync(c => c.Id != command.Id && c.NormalizedName == normalizedName, cancellationToken);
        
        BusinessException.ThrowIfTrue(
            existsCategory,
            ErrorMessages.Category.NameAlreadyExists,
            ErrorMessages.Exception.CategoryTitle);

        category!.UpdateDetails(command.Name, command.Description);
        _categoryRepository.Update(category);

        await _unitOfWork.CompleteAsync(cancellationToken);
        await _cacheService.RemoveAsync(CacheKeys.GetCategoryById(command.Id), cancellationToken);
    }
}