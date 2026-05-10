using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Exceptions;
using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Interfaces;
using e_Sat_Auction.Interfaces.Repositories;

namespace e_Sat_Auction.Features.Category.ActivateCategory;

using Models.Categories;

public sealed class ActivateCategoryCommandHandler : ICommandHandler<ActivateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        ICacheService cacheService,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ActivateCategoryCommand command, CancellationToken cancellationToken)
    {
        Category? category = await _categoryRepository.GetByIdAsync(command.Id, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(category, ErrorMessages.Category.EntityName, command.Id);

        category!.Activate();
        _categoryRepository.Update(category);
        
        await _unitOfWork.CompleteAsync(cancellationToken);
        await _cacheService.RemoveAsync(CacheKeys.GetCategoryById(command.Id), cancellationToken);
    }
}