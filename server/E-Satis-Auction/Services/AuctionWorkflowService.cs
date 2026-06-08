using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces;
using E_Satis_Auction.Dtos.Auction;
using E_Satis_Auction.Dtos.Commerce;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces.Repositories;
using E_Satis_Auction.Interfaces.Services;

namespace E_Satis_Auction.Services;

using AuctionEntity = Models.Commerce.Auction;
using AuctionInventoryReservationEntity = Models.Commerce.AuctionInventoryReservation;
using ItemEntity = Models.Items.Item;
using PaymentAttemptEntity = Models.Commerce.PaymentAttempt;
using ProductEntity = Models.Products.Product;
using ProductListingEntity = Models.Commerce.ProductListing;
using PurchaseOrderEntity = Models.Commerce.PurchaseOrder;

public sealed class AuctionWorkflowService : IAuctionWorkflowService
{
    private static readonly TimeSpan PaymentReservationTtl = TimeSpan.FromMinutes(15);

    private readonly IAuctionRepository _auctionRepository;
    private readonly IPaymentAttemptRepository _paymentAttemptRepository;
    private readonly IProductListingRepository _productListingRepository;
    private readonly IProductRepository _productRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IGenericRepository<AuctionInventoryReservationEntity> _auctionReservationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuctionRealtimeNotifier _notifier;

    public AuctionWorkflowService(
        IAuctionRepository auctionRepository,
        IPaymentAttemptRepository paymentAttemptRepository,
        IProductListingRepository productListingRepository,
        IProductRepository productRepository,
        IItemRepository itemRepository,
        IPurchaseOrderRepository purchaseOrderRepository,
        IGenericRepository<AuctionInventoryReservationEntity> auctionReservationRepository,
        IUnitOfWork unitOfWork,
        IAuctionRealtimeNotifier notifier)
    {
        _auctionRepository = auctionRepository;
        _paymentAttemptRepository = paymentAttemptRepository;
        _productListingRepository = productListingRepository;
        _productRepository = productRepository;
        _itemRepository = itemRepository;
        _purchaseOrderRepository = purchaseOrderRepository;
        _auctionReservationRepository = auctionReservationRepository;
        _unitOfWork = unitOfWork;
        _notifier = notifier;
    }

    public async Task<AuctionDetailDto> ActivateAuctionAsync(Guid auctionId, CancellationToken cancellationToken = default)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        AuctionEntity auction = await GetAuctionWithDetailsAsync(auctionId, true, cancellationToken);
        ProductListingProductEnrichmentDto product = await GetProductEnrichmentAsync(auction.ProductId, cancellationToken);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            auction.Activate(now);
            if (auction.Status == AuctionStatus.Active && auction.Reservations.All(reservation => reservation.Status != AuctionReservationStatus.Active))
            {
                await ReserveAuctionStockAsync(auction, cancellationToken);
            }

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        AuctionSnapshotDto snapshot = AuctionDtoMapper.ToSnapshotDto(auction, DateTimeOffset.UtcNow);
        await _notifier.BroadcastAuctionSnapshotAsync(snapshot, cancellationToken);

        return AuctionDtoMapper.ToDetailDto(auction, product);
    }

    public async Task<AuctionDetailDto> CancelAuctionAsync(Guid auctionId, CancellationToken cancellationToken = default)
    {
        AuctionEntity auction = await GetAuctionWithDetailsAsync(auctionId, true, cancellationToken);
        ProductListingProductEnrichmentDto product = await GetProductEnrichmentAsync(auction.ProductId, cancellationToken);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await ReleaseAuctionStockAsync(auction, cancellationToken);
            auction.Cancel();
            _auctionRepository.Update(auction);

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        AuctionSnapshotDto snapshot = AuctionDtoMapper.ToSnapshotDto(auction, DateTimeOffset.UtcNow);
        await _notifier.BroadcastAuctionCancelledAsync(snapshot, cancellationToken);

        return AuctionDtoMapper.ToDetailDto(auction, product);
    }

    public async Task<AuctionDetailDto> FinalizeAuctionAsync(Guid auctionId, CancellationToken cancellationToken = default)
    {
        AuctionEntity auction = await GetAuctionWithDetailsAsync(auctionId, true, cancellationToken);
        ProductListingProductEnrichmentDto product = await GetProductEnrichmentAsync(auction.ProductId, cancellationToken);

        await FinalizeLoadedAuctionAsync(auction, cancellationToken);

        return AuctionDtoMapper.ToDetailDto(auction, product);
    }

    public async Task<PaymentInitiationDto> InitiateWinnerPaymentAsync(Guid auctionId, string userId, string idempotencyKey, CancellationToken cancellationToken = default)
    {
        EnsureUserAndIdempotency(userId, idempotencyKey);

        PaymentAttemptEntity? existingPayment = await _paymentAttemptRepository.GetByIdempotencyKeyAsync(idempotencyKey, cancellationToken: cancellationToken);
        if (existingPayment is not null)
        {
            AuctionEntity? existingAuction = await _auctionRepository.GetByPaymentAttemptIdAsync(existingPayment.Id, cancellationToken: cancellationToken);
            BusinessException.ThrowIfTrue(
                existingAuction is null || existingAuction.Id != auctionId,
                ErrorMessages.Payment.IdempotencyConflict,
                ErrorMessages.Exception.CommerceTitle);

            PurchaseOrderEntity? existingOrder = await _purchaseOrderRepository.GetByIdWithDetailsAsync(existingPayment.PurchaseOrderId, cancellationToken: cancellationToken);
            NotFoundException.ThrowIfNull(existingOrder, ErrorMessages.PurchaseOrder.EntityName, existingPayment.PurchaseOrderId);
            return new PaymentInitiationDto(CommerceDtoMapper.ToPaymentAttemptDto(existingPayment), CommerceDtoMapper.ToOrderDetailDto(existingOrder!));
        }

        AuctionEntity auction = await GetAuctionWithDetailsAsync(auctionId, true, cancellationToken);
        ForbiddenAccessException.ThrowIfTrue(
            auction.WinningUserId != userId,
            ErrorMessages.Auction.WinnerOnly,
            ErrorMessages.Exception.UnauthorizedAccess);

        BusinessException.ThrowIfTrue(
            auction.Status is not AuctionStatus.PaymentPending,
            ErrorMessages.Auction.PaymentNotAvailable,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            auction.PaymentAttemptId.HasValue,
            ErrorMessages.Auction.PaymentAlreadyStarted,
            ErrorMessages.Exception.CommerceTitle);

        ProductEntity? product = await _productRepository.GetByIdAsync(auction.ProductId, cancellationToken: cancellationToken);
        NotFoundException.ThrowIfNull(product, ErrorMessages.Product.EntityName, auction.ProductId);

        decimal winningAmount = auction.WinningBidAmount ?? 0;
        BusinessException.ThrowIfTrue(winningAmount <= 0, ErrorMessages.Auction.WinnerRequired, ErrorMessages.Exception.CommerceTitle);

        List<Models.Commerce.AuctionInventoryReservation> activeReservations = auction.Reservations
            .Where(reservation => reservation.Status == AuctionReservationStatus.Active)
            .ToList();

        BusinessException.ThrowIfTrue(activeReservations.Count is 0, ErrorMessages.Auction.NoActiveReservation, ErrorMessages.Exception.CommerceTitle);

        PurchaseOrderEntity order = PurchaseOrderEntity.CreateForPayment(userId, OrderSource.AuctionWin, auction.Currency, idempotencyKey);
        Models.Commerce.PurchaseOrderLine line = order.AddLine(
            auction.ProductId,
            auction.ProductListingId,
            null,
            product!.Name,
            product.Sku,
            winningAmount,
            winningAmount,
            auction.Quantity,
            auction.Currency);

        order.ApplyOrderPricing(winningAmount * auction.Quantity, 0, 0, winningAmount * auction.Quantity, null, null);

        PaymentAttemptEntity payment = PaymentAttemptEntity.Create(
            order.Id,
            userId,
            order.TotalAmount,
            auction.Currency,
            idempotencyKey,
            DateTimeOffset.UtcNow.Add(PaymentReservationTtl));
        payment.EnterPaymentEntry();

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            foreach (Models.Commerce.AuctionInventoryReservation reservation in activeReservations)
            {
                line.AddAllocation(reservation.OriginalItemId, reservation.ReservedItemId, reservation.Quantity);
            }

            auction.MarkReservationsTransferred();
            auction.AttachPayment(order.Id, payment.Id);

            await _purchaseOrderRepository.AddAsync(order, cancellationToken);
            await _paymentAttemptRepository.AddAsync(payment, cancellationToken);
            _auctionRepository.Update(auction);

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        AuctionSnapshotDto snapshot = AuctionDtoMapper.ToSnapshotDto(auction, DateTimeOffset.UtcNow);
        await _notifier.BroadcastPaymentWindowStartedAsync(snapshot, payment.Id, payment.ExpiresAt, cancellationToken);

        return new PaymentInitiationDto(CommerceDtoMapper.ToPaymentAttemptDto(payment), CommerceDtoMapper.ToOrderDetailDto(order));
    }

    public async Task MarkAuctionPaymentSucceededAsync(AuctionEntity auction, CancellationToken cancellationToken = default)
    {
        auction.MarkPaymentSucceeded();
        _auctionRepository.Update(auction);
        await _notifier.BroadcastAuctionCompletedAsync(AuctionDtoMapper.ToSnapshotDto(auction, DateTimeOffset.UtcNow), cancellationToken);
    }

    public async Task MarkAuctionPaymentFailedAsync(AuctionEntity auction, CancellationToken cancellationToken = default)
    {
        auction.MarkPaymentFailed();
        _auctionRepository.Update(auction);
        await _notifier.BroadcastPaymentExpiredAsync(AuctionDtoMapper.ToSnapshotDto(auction, DateTimeOffset.UtcNow), cancellationToken);
    }

    private async Task FinalizeLoadedAuctionAsync(AuctionEntity auction, CancellationToken cancellationToken)
    {
        bool hasWinner;
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            auction.FinalizeAfterEnd(DateTimeOffset.UtcNow);
            hasWinner = auction.CurrentWinningBidId.HasValue;

            if (!hasWinner)
            {
                await ReleaseAuctionStockAsync(auction, cancellationToken);
            }

            _auctionRepository.Update(auction);
            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        AuctionSnapshotDto snapshot = AuctionDtoMapper.ToSnapshotDto(auction, DateTimeOffset.UtcNow);
        await _notifier.BroadcastAuctionEndedAsync(snapshot, cancellationToken);
        if (hasWinner)
        {
            await _notifier.BroadcastWinnerSelectedAsync(snapshot, AuctionDtoMapper.ToWinnerDto(auction), cancellationToken);
        }
    }

    private async Task ReserveAuctionStockAsync(AuctionEntity auction, CancellationToken cancellationToken)
    {
        ProductListingEntity? listing = await _productListingRepository.GetByIdAsync(auction.ProductListingId, enableTracking: true, cancellationToken);
        NotFoundException.ThrowIfNull(listing, ErrorMessages.ProductListing.EntityName, auction.ProductListingId);

        BusinessException.ThrowIfFalse(
            listing!.IsSellableAt(DateTimeOffset.UtcNow),
            ErrorMessages.ProductListing.NotSellable,
            ErrorMessages.Exception.CommerceTitle);

        List<ItemEntity> availableItems = await _itemRepository.GetAvailableItemsForProductAsync(
            auction.ProductId,
            listing.SourceFacilityId,
            enableTracking: true,
            cancellationToken);

        int availableQuantity = availableItems.Sum(item => item.Quantity);
        BusinessException.ThrowIfTrue(availableQuantity < auction.Quantity, ErrorMessages.PurchaseOrder.InsufficientStock, ErrorMessages.Exception.CommerceTitle);

        int remainingQuantity = auction.Quantity;
        foreach (ItemEntity item in availableItems)
        {
            if (remainingQuantity is 0)
            {
                break;
            }

            int reservedQuantity = Math.Min(item.Quantity, remainingQuantity);
            item.DecreaseQuantity(reservedQuantity, InventoryTransactionType.AuctionReserved, auction.Id);
            _itemRepository.Update(item);

            Dictionary<string, string> attributes = item.DynamicAttributes.ToDictionary(entry => entry.Key, entry => entry.Value);
            ItemEntity reservedItem = ItemEntity.CreateFromProduct(
                auction.ProductId,
                item.CategoryId,
                item.FacilityId,
                reservedQuantity,
                item.UnitOfMeasure,
                ItemStatus.Reserved,
                attributes,
                InventoryTransactionType.AuctionReserved,
                auction.Id);

            await _itemRepository.AddAsync(reservedItem, cancellationToken);
            AuctionInventoryReservationEntity reservation = auction.AddReservation(item.Id, reservedItem.Id, reservedQuantity);
            await _auctionReservationRepository.AddAsync(reservation, cancellationToken);
            remainingQuantity -= reservedQuantity;
        }

        BusinessException.ThrowIfTrue(remainingQuantity is not 0, ErrorMessages.PurchaseOrder.InsufficientStock, ErrorMessages.Exception.CommerceTitle);
    }

    private async Task ReleaseAuctionStockAsync(AuctionEntity auction, CancellationToken cancellationToken)
    {
        List<Models.Commerce.AuctionInventoryReservation> activeReservations = auction.Reservations
            .Where(reservation => reservation.Status == AuctionReservationStatus.Active)
            .ToList();

        if (activeReservations.Count is 0)
        {
            return;
        }

        List<Guid> itemIds = activeReservations
            .SelectMany(reservation => new[] { reservation.OriginalItemId, reservation.ReservedItemId })
            .Distinct()
            .ToList();

        List<ItemEntity> items = await _itemRepository.GetItemsByIdsAsync(itemIds, enableTracking: true, cancellationToken);
        Dictionary<Guid, ItemEntity> itemLookup = items.ToDictionary(item => item.Id);

        foreach (Models.Commerce.AuctionInventoryReservation reservation in activeReservations)
        {
            itemLookup.TryGetValue(reservation.OriginalItemId, out ItemEntity? originalItem);
            itemLookup.TryGetValue(reservation.ReservedItemId, out ItemEntity? reservedItem);

            NotFoundException.ThrowIfNull(originalItem, ErrorMessages.Item.EntityName, reservation.OriginalItemId);
            NotFoundException.ThrowIfNull(reservedItem, ErrorMessages.Item.EntityName, reservation.ReservedItemId);

            BusinessException.ThrowIfTrue(
                reservedItem!.Status is not ItemStatus.Reserved,
                ErrorMessages.Auction.InvalidReservedInventoryState,
                ErrorMessages.Exception.CommerceTitle);

            originalItem!.IncreaseQuantity(reservation.Quantity, InventoryTransactionType.AuctionReleased, auction.Id);
            reservedItem.Archive(InventoryTransactionType.AuctionReleased, auction.Id);
            _itemRepository.Update(originalItem);
            _itemRepository.Update(reservedItem);
        }

        auction.MarkReservationsReleased();
    }

    private async Task<AuctionEntity> GetAuctionWithDetailsAsync(Guid auctionId, bool enableTracking, CancellationToken cancellationToken)
    {
        AuctionEntity? auction = await _auctionRepository.GetByIdWithDetailsAsync(auctionId, enableTracking, cancellationToken);
        NotFoundException.ThrowIfNull(auction, ErrorMessages.Auction.EntityName, auctionId);
        return auction!;
    }

    private async Task<ProductListingProductEnrichmentDto> GetProductEnrichmentAsync(Guid productId, CancellationToken cancellationToken)
    {
        Dictionary<Guid, ProductListingProductEnrichmentDto> products = await _productRepository.GetProductListingEnrichmentsByIdsAsync([productId], cancellationToken);
        products.TryGetValue(productId, out ProductListingProductEnrichmentDto? product);
        NotFoundException.ThrowIfNull(product, ErrorMessages.Product.EntityName, productId);
        return product!;
    }

    private static void EnsureUserAndIdempotency(string userId, string idempotencyKey)
    {
        ForbiddenAccessException.ThrowIfTrue(string.IsNullOrWhiteSpace(userId), ErrorMessages.Auth.UnauthorizedAccess, ErrorMessages.Exception.UnauthorizedAccess);
        BusinessException.ThrowIfNullOrWhiteSpace(idempotencyKey, ErrorMessages.Payment.IdempotencyKeyRequired, ErrorMessages.Exception.CommerceTitle);
    }
}
