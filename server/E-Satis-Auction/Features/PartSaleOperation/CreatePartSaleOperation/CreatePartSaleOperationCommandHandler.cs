using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.PartSaleOperation.CreatePartSaleOperation;

using ItemEntity = Models.Items.Item;

public sealed class CreatePartSaleOperationCommandHandler : ICommandHandler<CreatePartSaleOperationCommand, PartSaleOperationDto>
{
    private readonly IItemRepository _itemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IPartSaleOperationRepository _partSaleOperationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePartSaleOperationCommandHandler(
        IItemRepository itemRepository,
        IProductRepository productRepository,
        IPartSaleOperationRepository partSaleOperationRepository,
        IUnitOfWork unitOfWork)
    {
        _itemRepository = itemRepository;
        _productRepository = productRepository;
        _partSaleOperationRepository = partSaleOperationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PartSaleOperationDto> Handle(CreatePartSaleOperationCommand command, CancellationToken cancellationToken)
    {
        ItemEntity? sourceItem = await _itemRepository.GetByIdAsync(command.Payload.SourceItemId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(sourceItem, ErrorMessages.Item.EntityName, command.Payload.SourceItemId);

        Models.Products.Product? partProduct = await _productRepository.GetByIdAsync(command.Payload.ProductId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(partProduct, ErrorMessages.Product.EntityName, command.Payload.ProductId);

        BusinessException.ThrowIfFalse(partProduct!.IsActive, ErrorMessages.Product.ProductNotAvailable, ErrorMessages.Exception.ProductTitle);
        BusinessException.ThrowIfTrue(sourceItem!.Status is not ItemStatus.Available, ErrorMessages.Dispatch.ItemStatusInvalid, ErrorMessages.Exception.InventoryTitle);
        BusinessException.ThrowIfTrue(sourceItem.Quantity < command.Payload.Quantity, ErrorMessages.Dispatch.InsufficientStock, ErrorMessages.Exception.InventoryTitle);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            sourceItem.DecreaseQuantity(command.Payload.Quantity, InventoryTransactionType.PartSaleConsumed, sourceItem.Id);
            _itemRepository.Update(sourceItem);

            ItemEntity partItem = ItemEntity.CreateFromProduct(
                partProduct.Id,
                partProduct.CategoryId,
                command.Payload.FacilityId,
                command.Payload.Quantity,
                command.Payload.UnitOfMeasure,
                ItemStatus.Available,
                command.Payload.DynamicAttributes ?? [],
                InventoryTransactionType.PartSaleCreated,
                sourceItem.Id,
                sourceItem.Id);

            await _itemRepository.AddAsync(partItem, cancellationToken);

            Models.Commerce.PartSaleOperation operation = Models.Commerce.PartSaleOperation.Create(
                sourceItem.Id,
                partItem.Id,
                partProduct.Id,
                command.Payload.FacilityId,
                command.Payload.Quantity,
                command.Payload.UnitOfMeasure,
                command.Payload.Notes);

            await _partSaleOperationRepository.AddAsync(operation, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return CommerceDtoMapper.ToPartSaleOperationDto(operation);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
