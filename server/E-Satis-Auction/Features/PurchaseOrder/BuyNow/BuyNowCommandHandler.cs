using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Interfaces.Repositories;

namespace E_Satis_Auction.Features.PurchaseOrder.BuyNow;

using CampaignEntity = Models.Commerce.Campaign;
using ItemEntity = Models.Items.Item;
using ProductEntity = Models.Products.Product;
using ProductListingEntity = Models.Commerce.ProductListing;
using PurchaseOrderEntity = Models.Commerce.PurchaseOrder;

public sealed class BuyNowCommandHandler : ICommandHandler<BuyNowCommand, OrderDetailDto>
{
    private readonly IProductListingRepository _productListingRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICampaignRepository _campaignRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public BuyNowCommandHandler(
        IProductListingRepository productListingRepository,
        IProductRepository productRepository,
        ICampaignRepository campaignRepository,
        IItemRepository itemRepository,
        IPurchaseOrderRepository purchaseOrderRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _productListingRepository = productListingRepository;
        _productRepository = productRepository;
        _campaignRepository = campaignRepository;
        _itemRepository = itemRepository;
        _purchaseOrderRepository = purchaseOrderRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<OrderDetailDto> Handle(BuyNowCommand command, CancellationToken cancellationToken)
    {
        string userId = _currentUserService.UserId;
        ForbiddenAccessException.ThrowIfTrue(
            string.IsNullOrWhiteSpace(userId),
            ErrorMessages.Auth.UnauthorizedAccess,
            ErrorMessages.Exception.UnauthorizedAccess);

        DateTimeOffset now = DateTimeOffset.UtcNow;
        ProductListingEntity? listing = await _productListingRepository.GetByIdAsync(command.ProductListingId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(listing, ErrorMessages.ProductListing.EntityName, command.ProductListingId);

        BusinessException.ThrowIfFalse(
            listing!.IsSellableAt(now),
            ErrorMessages.ProductListing.NotSellable,
            ErrorMessages.Exception.CommerceTitle);

        ProductEntity? product = await _productRepository.GetByIdAsync(listing.ProductId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(product, ErrorMessages.Product.EntityName, listing.ProductId);

        BusinessException.ThrowIfFalse(
            product!.IsActive,
            ErrorMessages.Product.ProductNotAvailable,
            ErrorMessages.Exception.ProductTitle);

        CampaignEntity? campaign = await LoadCampaignAsync(command.CampaignId, listing.ProductId, now, cancellationToken);
        decimal discountedUnitPrice = campaign is null
            ? listing.SalePrice
            : campaign.ApplyDiscount(listing.SalePrice, listing.Currency);

        List<ItemEntity> availableItems = await _itemRepository.GetAvailableItemsForProductAsync(
            listing.ProductId,
            listing.SourceFacilityId,
            enableTracking: true,
            cancellationToken);

        int availableQuantity = availableItems.Sum(item => item.Quantity);
        BusinessException.ThrowIfTrue(
            availableQuantity < command.Quantity,
            ErrorMessages.PurchaseOrder.InsufficientStock,
            ErrorMessages.Exception.CommerceTitle);

        PurchaseOrderEntity order = PurchaseOrderEntity.Create(userId, OrderSource.DirectPurchase, listing.Currency);
        Models.Commerce.PurchaseOrderLine line = order.AddLine(
            product.Id,
            listing.Id,
            campaign?.Id,
            product.Name,
            product.Sku,
            listing.SalePrice,
            discountedUnitPrice,
            command.Quantity,
            listing.Currency);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            int remainingQuantity = command.Quantity;
            foreach (ItemEntity item in availableItems)
            {
                if (remainingQuantity is 0)
                {
                    break;
                }

                int reservedQuantity = Math.Min(item.Quantity, remainingQuantity);
                item.DecreaseQuantity(reservedQuantity, InventoryTransactionType.PurchaseReserved, order.Id);
                _itemRepository.Update(item);

                Dictionary<string, string> attributes = item.DynamicAttributes.ToDictionary(entry => entry.Key, entry => entry.Value);
                ItemEntity reservedItem = ItemEntity.CreateFromProduct(
                    product.Id,
                    item.CategoryId,
                    item.FacilityId,
                    reservedQuantity,
                    item.UnitOfMeasure,
                    ItemStatus.Reserved,
                    attributes,
                    InventoryTransactionType.PurchaseReserved,
                    order.Id);

                await _itemRepository.AddAsync(reservedItem, cancellationToken);
                line.AddAllocation(item.Id, reservedItem.Id, reservedQuantity);
                remainingQuantity -= reservedQuantity;
            }

            await _purchaseOrderRepository.AddAsync(order, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return CommerceDtoMapper.ToOrderDetailDto(order);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task<CampaignEntity?> LoadCampaignAsync(
        Guid? campaignId,
        Guid productId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        if (!campaignId.HasValue)
        {
            return null;
        }

        CampaignEntity? campaign = await _campaignRepository.GetWithProductsByIdAsync(campaignId.Value, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(campaign, ErrorMessages.Campaign.EntityName, campaignId.Value);

        BusinessException.ThrowIfFalse(
            campaign!.IsApplicableTo(productId, now),
            ErrorMessages.Campaign.NotApplicable,
            ErrorMessages.Exception.CommerceTitle);

        return campaign;
    }
}
