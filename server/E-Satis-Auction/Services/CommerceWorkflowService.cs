using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Interfaces.Services;

namespace E_Satis_Auction.Services;

using CampaignEntity = Models.Commerce.Campaign;
using ItemEntity = Models.Items.Item;
using ProductEntity = Models.Products.Product;
using ProductListingEntity = Models.Commerce.ProductListing;
using PurchaseOrderEntity = Models.Commerce.PurchaseOrder;
using ShoppingCartEntity = Models.Commerce.ShoppingCart;
using PaymentAttemptEntity = Models.Commerce.PaymentAttempt;

public sealed class CommerceWorkflowService : ICommerceWorkflowService
{
    private const decimal DefaultShippingAmount = 49.90m;
    private static readonly TimeSpan ReservationTtl = TimeSpan.FromMinutes(15);

    private readonly IShoppingCartRepository _cartRepository;
    private readonly IPaymentAttemptRepository _paymentAttemptRepository;
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IProductListingRepository _productListingRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICampaignRepository _campaignRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CommerceWorkflowService(
        IShoppingCartRepository cartRepository,
        IPaymentAttemptRepository paymentAttemptRepository,
        IPurchaseOrderRepository purchaseOrderRepository,
        IProductListingRepository productListingRepository,
        IProductRepository productRepository,
        ICampaignRepository campaignRepository,
        IItemRepository itemRepository,
        IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _paymentAttemptRepository = paymentAttemptRepository;
        _purchaseOrderRepository = purchaseOrderRepository;
        _productListingRepository = productListingRepository;
        _productRepository = productRepository;
        _campaignRepository = campaignRepository;
        _itemRepository = itemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CartPricePreviewDto> PreviewCartAsync(ShoppingCartEntity cart, CancellationToken cancellationToken = default)
    {
        PriceContext context = await BuildPriceContextAsync(cart.ProductListingId, cart.Quantity, cart.AppliedCouponCampaignId, cancellationToken);
        return context.Preview;
    }

    public async Task<PaymentInitiationDto> InitiatePaymentFromCartAsync(string userId, string idempotencyKey, CancellationToken cancellationToken = default)
    {
        EnsureUserAndIdempotency(userId, idempotencyKey);

        PaymentAttemptEntity? existingPayment = await _paymentAttemptRepository.GetByIdempotencyKeyAsync(idempotencyKey, cancellationToken: cancellationToken);
        if (existingPayment is not null)
        {
            PurchaseOrderEntity? existingOrder = await _purchaseOrderRepository.GetByIdWithDetailsAsync(existingPayment.PurchaseOrderId, cancellationToken: cancellationToken);
            NotFoundException.ThrowIfNull(existingOrder, ErrorMessages.PurchaseOrder.EntityName, existingPayment.PurchaseOrderId);
            return new PaymentInitiationDto(CommerceDtoMapper.ToPaymentAttemptDto(existingPayment), CommerceDtoMapper.ToOrderDetailDto(existingOrder!));
        }

        ShoppingCartEntity? cart = await _cartRepository.GetActiveByUserIdAsync(userId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(cart, ErrorMessages.Cart.EntityName, userId);

        PriceContext context = await BuildPriceContextAsync(cart!.ProductListingId, cart.Quantity, cart.AppliedCouponCampaignId, cancellationToken);

        List<ItemEntity> availableItems = await _itemRepository.GetAvailableItemsForProductAsync(
            context.Listing.ProductId,
            context.Listing.SourceFacilityId,
            enableTracking: true,
            cancellationToken);

        int availableQuantity = availableItems.Sum(item => item.Quantity);
        BusinessException.ThrowIfTrue(availableQuantity < cart.Quantity, ErrorMessages.PurchaseOrder.InsufficientStock, ErrorMessages.Exception.CommerceTitle);

        PurchaseOrderEntity order = PurchaseOrderEntity.CreateForPayment(userId, OrderSource.DirectPurchase, context.Listing.Currency, idempotencyKey);
        Models.Commerce.PurchaseOrderLine line = order.AddLine(
            context.Product.Id,
            context.Listing.Id,
            context.Preview.AppliedLineCampaignId,
            context.Product.Name,
            context.Product.Sku,
            context.Listing.SalePrice,
            context.Preview.DiscountedUnitPrice,
            cart.Quantity,
            context.Listing.Currency,
            context.Preview.LineDiscountAmount,
            context.Preview.AppliedCouponCampaignId,
            context.Preview.CouponDiscountAmount);

        order.ApplyOrderPricing(
            context.Preview.SubtotalAmount,
            context.Preview.DiscountAmount,
            context.Preview.ShippingAmount,
            context.Preview.TotalAmount,
            context.Preview.AppliedCouponCampaignId,
            context.Preview.AppliedFreeShippingCampaignId);

        PaymentAttemptEntity payment = PaymentAttemptEntity.Create(
            order.Id,
            userId,
            context.Preview.TotalAmount,
            context.Listing.Currency,
            idempotencyKey,
            DateTimeOffset.UtcNow.Add(ReservationTtl));
        payment.EnterPaymentEntry();

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await ReserveStockAsync(availableItems, context.Product, order.Id, line, cart.Quantity, cancellationToken);
            cart.UpdatePreview(
                context.Preview.SubtotalAmount,
                context.Preview.DiscountAmount,
                context.Preview.ShippingAmount,
                context.Preview.TotalAmount,
                context.Listing.Currency);
            cart.MarkCheckedOut();

            await _purchaseOrderRepository.AddAsync(order, cancellationToken);
            await _paymentAttemptRepository.AddAsync(payment, cancellationToken);
            _cartRepository.Update(cart);

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new PaymentInitiationDto(CommerceDtoMapper.ToPaymentAttemptDto(payment), CommerceDtoMapper.ToOrderDetailDto(order));
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<PaymentAttemptDto> ConfirmPaymentAsync(string userId, Guid paymentAttemptId, string idempotencyKey, CancellationToken cancellationToken = default)
    {
        EnsureUserAndIdempotency(userId, idempotencyKey);

        PaymentAttemptEntity? payment = await _paymentAttemptRepository.GetByIdForUserAsync(paymentAttemptId, userId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(payment, ErrorMessages.Payment.EntityName, paymentAttemptId);

        BusinessException.ThrowIfFalse(
            string.Equals(payment!.IdempotencyKey, idempotencyKey, StringComparison.Ordinal),
            ErrorMessages.Payment.IdempotencyConflict,
            ErrorMessages.Exception.CommerceTitle);

        if (payment.Status is PaymentStatus.Succeeded)
        {
            return CommerceDtoMapper.ToPaymentAttemptDto(payment);
        }

        BusinessException.ThrowIfTrue(payment.ExpiresAt <= DateTimeOffset.UtcNow, ErrorMessages.Payment.Expired, ErrorMessages.Exception.CommerceTitle);

        PurchaseOrderEntity? order = await _purchaseOrderRepository.GetByIdWithDetailsAsync(payment.PurchaseOrderId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(order, ErrorMessages.PurchaseOrder.EntityName, payment.PurchaseOrderId);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            payment.MarkSucceeded();
            order!.MarkPaymentSucceeded();
            _paymentAttemptRepository.Update(payment);
            _purchaseOrderRepository.Update(order);

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return CommerceDtoMapper.ToPaymentAttemptDto(payment);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<PaymentAttemptDto> FailPaymentAsync(string userId, Guid paymentAttemptId, string idempotencyKey, string reason, CancellationToken cancellationToken = default)
    {
        EnsureUserAndIdempotency(userId, idempotencyKey);

        PaymentAttemptEntity? payment = await _paymentAttemptRepository.GetByIdForUserAsync(paymentAttemptId, userId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(payment, ErrorMessages.Payment.EntityName, paymentAttemptId);

        BusinessException.ThrowIfFalse(
            string.Equals(payment!.IdempotencyKey, idempotencyKey, StringComparison.Ordinal),
            ErrorMessages.Payment.IdempotencyConflict,
            ErrorMessages.Exception.CommerceTitle);

        if (payment.Status is PaymentStatus.Failed)
        {
            return CommerceDtoMapper.ToPaymentAttemptDto(payment);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            payment.MarkFailed(reason);
            await ReleaseReservedStockAsync(payment.PurchaseOrderId, cancellationToken);
            _paymentAttemptRepository.Update(payment);

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return CommerceDtoMapper.ToPaymentAttemptDto(payment);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task ExpirePaymentAsync(PaymentAttemptEntity paymentAttempt, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            paymentAttempt.MarkExpired(DateTimeOffset.UtcNow);
            await ReleaseReservedStockAsync(paymentAttempt.PurchaseOrderId, cancellationToken);
            _paymentAttemptRepository.Update(paymentAttempt);

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task<PriceContext> BuildPriceContextAsync(Guid productListingId, int quantity, Guid? couponCampaignId, CancellationToken cancellationToken)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        ProductListingEntity? listing = await _productListingRepository.GetByIdAsync(productListingId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(listing, ErrorMessages.ProductListing.EntityName, productListingId);

        BusinessException.ThrowIfFalse(listing!.IsSellableAt(now), ErrorMessages.ProductListing.NotSellable, ErrorMessages.Exception.CommerceTitle);

        ProductEntity? product = await _productRepository.GetByIdAsync(listing.ProductId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(product, ErrorMessages.Product.EntityName, listing.ProductId);

        BusinessException.ThrowIfFalse(product!.IsActive, ErrorMessages.Product.ProductNotAvailable, ErrorMessages.Exception.ProductTitle);

        List<CampaignEntity> lineCampaigns = await _campaignRepository.GetActiveLineCampaignsAsync(now, cancellationToken);
        CampaignEntity? bestLineCampaign = lineCampaigns
            .Where(campaign => campaign.IsLineApplicableTo(product.Id, listing.Id, product.CategoryId, now))
            .Select(campaign => new
            {
                Campaign = campaign,
                DiscountedUnitPrice = campaign.ApplyDiscount(listing.SalePrice, listing.Currency)
            })
            .OrderBy(candidate => candidate.DiscountedUnitPrice)
            .FirstOrDefault()
            ?.Campaign;

        decimal discountedUnitPrice = bestLineCampaign?.ApplyDiscount(listing.SalePrice, listing.Currency) ?? listing.SalePrice;
        decimal lineDiscountAmount = (listing.SalePrice - discountedUnitPrice) * quantity;
        decimal discountedSubtotal = discountedUnitPrice * quantity;

        CampaignEntity? coupon = null;
        decimal couponDiscountAmount = 0;
        if (couponCampaignId.HasValue)
        {
            coupon = await _campaignRepository.GetByIdAsync(couponCampaignId.Value, cancellationToken: cancellationToken);
            NotFoundException.ThrowIfNull(coupon, ErrorMessages.Campaign.EntityName, couponCampaignId.Value);

            BusinessException.ThrowIfTrue(
                coupon!.Scope is not CampaignScope.CartOrder || coupon.Status is not CampaignStatus.Active,
                ErrorMessages.Campaign.NotApplicable,
                ErrorMessages.Exception.CommerceTitle);

            BusinessException.ThrowIfFalse(
                coupon.IsCouponApplicable(coupon.CouponCode ?? string.Empty, discountedSubtotal, listing.Currency, now),
                ErrorMessages.Campaign.NotApplicable,
                ErrorMessages.Exception.CommerceTitle);

            couponDiscountAmount = coupon.CalculateDiscountAmount(discountedSubtotal, listing.Currency);
        }

        decimal subtotalAfterCoupon = discountedSubtotal - couponDiscountAmount;
        CampaignEntity? freeShipping = (await _campaignRepository.GetActiveFreeShippingCampaignsAsync(now, cancellationToken))
            .Where(campaign => campaign.IsFreeShippingApplicable(subtotalAfterCoupon, listing.Currency, now))
            .OrderByDescending(campaign => campaign.MinimumOrderAmount ?? 0)
            .FirstOrDefault();

        decimal shippingAmount = freeShipping is null ? DefaultShippingAmount : 0;
        decimal totalAmount = subtotalAfterCoupon + shippingAmount;
        decimal subtotalAmount = listing.SalePrice * quantity;
        decimal discountAmount = lineDiscountAmount + couponDiscountAmount;

        CartPricePreviewDto preview = new(
            listing.Id,
            quantity,
            listing.SalePrice,
            discountedUnitPrice,
            lineDiscountAmount,
            couponDiscountAmount,
            subtotalAmount,
            discountAmount,
            shippingAmount,
            totalAmount,
            listing.Currency,
            bestLineCampaign?.Id,
            coupon?.Id,
            freeShipping?.Id);

        return new PriceContext(listing, product, preview);
    }

    private async Task ReserveStockAsync(
        List<ItemEntity> availableItems,
        ProductEntity product,
        Guid orderId,
        Models.Commerce.PurchaseOrderLine line,
        int quantity,
        CancellationToken cancellationToken)
    {
        int remainingQuantity = quantity;
        foreach (ItemEntity item in availableItems)
        {
            if (remainingQuantity is 0)
            {
                break;
            }

            int reservedQuantity = Math.Min(item.Quantity, remainingQuantity);
            item.DecreaseQuantity(reservedQuantity, InventoryTransactionType.PurchaseReserved, orderId);
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
                orderId);

            await _itemRepository.AddAsync(reservedItem, cancellationToken);
            line.AddAllocation(item.Id, reservedItem.Id, reservedQuantity);
            remainingQuantity -= reservedQuantity;
        }

        BusinessException.ThrowIfTrue(
            remainingQuantity is not 0,
            ErrorMessages.PurchaseOrder.InsufficientStock,
            ErrorMessages.Exception.CommerceTitle);
    }

    private async Task ReleaseReservedStockAsync(Guid purchaseOrderId, CancellationToken cancellationToken)
    {
        PurchaseOrderEntity? order = await _purchaseOrderRepository.GetByIdWithDetailsAsync(purchaseOrderId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(order, ErrorMessages.PurchaseOrder.EntityName, purchaseOrderId);

        List<Guid> itemIds = order!.Lines
            .SelectMany(line => line.Allocations)
            .SelectMany(allocation => new[] { allocation.OriginalItemId, allocation.ReservedItemId })
            .Distinct()
            .ToList();

        List<ItemEntity> items = await _itemRepository.GetItemsByIdsAsync(itemIds, enableTracking: true, cancellationToken);
        Dictionary<Guid, ItemEntity> itemLookup = items.ToDictionary(item => item.Id);

        foreach (Models.Commerce.PurchaseOrderLineAllocation allocation in order.Lines.SelectMany(line => line.Allocations))
        {
            itemLookup.TryGetValue(allocation.OriginalItemId, out ItemEntity? originalItem);
            itemLookup.TryGetValue(allocation.ReservedItemId, out ItemEntity? reservedItem);

            NotFoundException.ThrowIfNull(originalItem, ErrorMessages.Item.EntityName, allocation.OriginalItemId);
            NotFoundException.ThrowIfNull(reservedItem, ErrorMessages.Item.EntityName, allocation.ReservedItemId);

            BusinessException.ThrowIfTrue(reservedItem!.Status is not ItemStatus.Reserved, ErrorMessages.PurchaseOrder.InvalidReservedInventoryState, ErrorMessages.Exception.CommerceTitle);

            originalItem!.IncreaseQuantity(allocation.Quantity, InventoryTransactionType.PurchaseReleased, order.Id);
            reservedItem.Archive(InventoryTransactionType.PurchaseReleased, order.Id);
            _itemRepository.Update(originalItem);
            _itemRepository.Update(reservedItem);
        }

        if (order.Status is PurchaseOrderStatus.PaymentPending)
        {
            order.CancelPaymentPending();
            _purchaseOrderRepository.Update(order);
        }
    }

    private static void EnsureUserAndIdempotency(string userId, string idempotencyKey)
    {
        ForbiddenAccessException.ThrowIfTrue(string.IsNullOrWhiteSpace(userId), ErrorMessages.Auth.UnauthorizedAccess, ErrorMessages.Exception.UnauthorizedAccess);
        BusinessException.ThrowIfNullOrWhiteSpace(idempotencyKey, ErrorMessages.Payment.IdempotencyKeyRequired, ErrorMessages.Exception.CommerceTitle);
    }

    private sealed record PriceContext(ProductListingEntity Listing, ProductEntity Product, CartPricePreviewDto Preview);
}
