using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Exceptions;
using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Interfaces;
using e_Sat_Auction.Interfaces.Repositories;

namespace e_Sat_Auction.Features.Product.DeactivateProduct;

using Models.Products;

public class DeactivateProductCommandHandler : ICommandHandler<DeactivateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateProductCommandHandler(
        IProductRepository productRepository,
        ICacheService cacheService,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeactivateProductCommand command, CancellationToken cancellationToken)
    {
        Product? product = await _productRepository.GetByIdAsync(command.Id, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(product, ErrorMessages.Product.EntityName, command.Id);

        product!.Deactivate();
        _productRepository.Update(product);
        
        await _unitOfWork.CompleteAsync(cancellationToken);
        await _cacheService.RemoveAsync(CacheKeys.GetProductById(command.Id), cancellationToken);
    }
}