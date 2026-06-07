using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.ProductListing.DeleteProductListing;

using ProductListingEntity = Models.Commerce.ProductListing;

public sealed class DeleteProductListingCommandHandler : ICommandHandler<DeleteProductListingCommand>
{
    private readonly IProductListingRepository _productListingRepository;
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductListingCommandHandler(
        IProductListingRepository productListingRepository,
        IPurchaseOrderRepository purchaseOrderRepository,
        IUnitOfWork unitOfWork)
    {
        _productListingRepository = productListingRepository;
        _purchaseOrderRepository = purchaseOrderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteProductListingCommand command, CancellationToken cancellationToken)
    {
        ProductListingEntity? listing = await _productListingRepository.GetByIdAsync(command.Id, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(listing, ErrorMessages.ProductListing.EntityName, command.Id);

        bool hasOrders = await _purchaseOrderRepository.HasLineForProductListingAsync(command.Id, cancellationToken);
        BusinessException.ThrowIfTrue(
            hasOrders,
            ErrorMessages.ProductListing.CannotDeleteWithOrders,
            ErrorMessages.Exception.CommerceTitle);

        _productListingRepository.Delete(listing!);
        await _unitOfWork.CompleteAsync(cancellationToken);
    }
}
