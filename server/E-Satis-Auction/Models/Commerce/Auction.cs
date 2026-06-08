using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Models.Commerce;

public sealed class Auction : BaseEntity
{
    public Guid ProductListingId { get; private set; }
    public Guid ProductId { get; private set; }
    public string? SellerUserId { get; private set; }
    public decimal StartingPrice { get; private set; }
    public decimal CurrentPrice { get; private set; }
    public decimal MinimumBidIncrement { get; private set; }
    public DateTimeOffset StartsAt { get; private set; }
    public DateTimeOffset EndsAt { get; private set; }
    public DateTimeOffset OriginalEndsAt { get; private set; }
    public AuctionStatus Status { get; private set; }
    public Guid? CurrentWinningBidId { get; private set; }
    public string? WinningUserId { get; private set; }
    public decimal? WinningBidAmount { get; private set; }
    public Guid? PurchaseOrderId { get; private set; }
    public Guid? PaymentAttemptId { get; private set; }
    public int Quantity { get; private set; }
    public string Currency { get; private set; }
    public decimal WaitingFeeAmount { get; private set; }
    public decimal ServiceFeeAmount { get; private set; }
    public decimal SellerPayoutAmount { get; private set; }
    public decimal PlatformRevenueAmount { get; private set; }
    public uint Version { get; private set; }

    private readonly List<AuctionBid> _bids = [];
    public IReadOnlyCollection<AuctionBid> Bids => _bids;

    private readonly List<AuctionInventoryReservation> _reservations = [];
    public IReadOnlyCollection<AuctionInventoryReservation> Reservations => _reservations;

    private Auction()
    {
        Currency = string.Empty;
        Status = AuctionStatus.Draft;
    }

    public static Auction Create(
        Guid productListingId,
        Guid productId,
        string? sellerUserId,
        decimal startingPrice,
        decimal minimumBidIncrement,
        DateTimeOffset startsAt,
        DateTimeOffset endsAt,
        int quantity,
        string currency)
    {
        ValidateCore(productListingId, productId, startingPrice, minimumBidIncrement, startsAt, endsAt, quantity, currency);

        return new Auction
        {
            ProductListingId = productListingId,
            ProductId = productId,
            SellerUserId = string.IsNullOrWhiteSpace(sellerUserId) ? null : sellerUserId.Trim(),
            StartingPrice = startingPrice,
            CurrentPrice = startingPrice,
            MinimumBidIncrement = minimumBidIncrement,
            StartsAt = startsAt,
            EndsAt = endsAt,
            OriginalEndsAt = endsAt,
            Quantity = quantity,
            Currency = currency.Trim().ToUpperInvariant(),
            Status = AuctionStatus.Draft
        };
    }

    public void Update(
        decimal startingPrice,
        decimal minimumBidIncrement,
        DateTimeOffset startsAt,
        DateTimeOffset endsAt,
        int quantity,
        string currency,
        string? sellerUserId)
    {
        BusinessException.ThrowIfTrue(
            Status is not (AuctionStatus.Draft or AuctionStatus.Scheduled),
            ErrorMessages.Auction.CannotUpdateInCurrentStatus,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            _bids.Count > 0,
            ErrorMessages.Auction.CannotUpdateAfterBids,
            ErrorMessages.Exception.CommerceTitle);

        ValidateCore(ProductListingId, ProductId, startingPrice, minimumBidIncrement, startsAt, endsAt, quantity, currency);

        StartingPrice = startingPrice;
        CurrentPrice = startingPrice;
        MinimumBidIncrement = minimumBidIncrement;
        StartsAt = startsAt;
        EndsAt = endsAt;
        OriginalEndsAt = endsAt;
        Quantity = quantity;
        Currency = currency.Trim().ToUpperInvariant();
        SellerUserId = string.IsNullOrWhiteSpace(sellerUserId) ? null : sellerUserId.Trim();
    }

    public void Schedule(DateTimeOffset startsAt, DateTimeOffset endsAt)
    {
        BusinessException.ThrowIfTrue(
            Status is not (AuctionStatus.Draft or AuctionStatus.Scheduled),
            ErrorMessages.Auction.InvalidStatusTransition,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            startsAt >= endsAt,
            ErrorMessages.Auction.InvalidDateRange,
            ErrorMessages.Exception.CommerceTitle);

        StartsAt = startsAt;
        EndsAt = endsAt;
        OriginalEndsAt = endsAt;
        Status = AuctionStatus.Scheduled;
    }

    public void Activate(DateTimeOffset now)
    {
        BusinessException.ThrowIfTrue(
            Status is not AuctionStatus.Scheduled,
            ErrorMessages.Auction.InvalidStatusTransition,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            StartsAt > now,
            ErrorMessages.Auction.NotStarted,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            EndsAt <= now,
            ErrorMessages.Auction.AlreadyEnded,
            ErrorMessages.Exception.CommerceTitle);

        Status = AuctionStatus.Active;
    }

    public AuctionBid AcceptBid(
        string bidderUserId,
        decimal amount,
        string idempotencyKey,
        DateTimeOffset now,
        TimeSpan antiSnipeWindow,
        TimeSpan antiSnipeExtension,
        out bool wasExtended)
    {
        wasExtended = false;

        BusinessException.ThrowIfTrue(
            Status is not AuctionStatus.Active,
            ErrorMessages.Auction.NotActive,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            EndsAt <= now,
            ErrorMessages.Auction.AlreadyEnded,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            !string.IsNullOrWhiteSpace(SellerUserId) && SellerUserId == bidderUserId,
            ErrorMessages.Auction.SellerCannotBid,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            amount < MinimumNextBid,
            ErrorMessages.Auction.BidBelowMinimumNextBid,
            ErrorMessages.Exception.CommerceTitle);

        AuctionBid bid = AuctionBid.Create(Id, bidderUserId, amount, idempotencyKey);
        _bids.Add(bid);

        CurrentPrice = amount;
        CurrentWinningBidId = bid.Id;
        WinningUserId = bidderUserId;
        WinningBidAmount = amount;

        if (EndsAt - now <= antiSnipeWindow)
        {
            EndsAt = EndsAt.Add(antiSnipeExtension);
            wasExtended = true;
        }

        return bid;
    }

    public decimal MinimumNextBid => CurrentWinningBidId.HasValue
        ? CurrentPrice + MinimumBidIncrement
        : CurrentPrice;

    public AuctionInventoryReservation AddReservation(Guid originalItemId, Guid reservedItemId, int quantity)
    {
        AuctionInventoryReservation reservation = AuctionInventoryReservation.Create(Id, originalItemId, reservedItemId, quantity);
        _reservations.Add(reservation);
        return reservation;
    }

    public void MarkReservationsReleased()
    {
        foreach (AuctionInventoryReservation reservation in _reservations.Where(reservation => reservation.Status == AuctionReservationStatus.Active))
        {
            reservation.MarkReleased();
        }
    }

    public void MarkReservationsTransferred()
    {
        foreach (AuctionInventoryReservation reservation in _reservations.Where(reservation => reservation.Status == AuctionReservationStatus.Active))
        {
            reservation.MarkTransferredToOrder();
        }
    }

    public void Cancel()
    {
        BusinessException.ThrowIfTrue(
            Status is AuctionStatus.Completed or AuctionStatus.Cancelled,
            ErrorMessages.Auction.InvalidStatusTransition,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            PaymentAttemptId.HasValue,
            ErrorMessages.Auction.PaymentAlreadyStarted,
            ErrorMessages.Exception.CommerceTitle);

        Status = AuctionStatus.Cancelled;
    }

    public void FinalizeAfterEnd(DateTimeOffset now)
    {
        BusinessException.ThrowIfTrue(
            Status is not AuctionStatus.Active,
            ErrorMessages.Auction.NotActive,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            EndsAt > now,
            ErrorMessages.Auction.NotEnded,
            ErrorMessages.Exception.CommerceTitle);

        if (CurrentWinningBidId is null)
        {
            Status = AuctionStatus.Relistable;
            return;
        }

        Status = AuctionStatus.PaymentPending;
        CalculateSettlement();
    }

    public void AttachPayment(Guid purchaseOrderId, Guid paymentAttemptId)
    {
        BusinessException.ThrowIfTrue(
            Status is not AuctionStatus.PaymentPending,
            ErrorMessages.Auction.PaymentNotAvailable,
            ErrorMessages.Exception.CommerceTitle);

        PurchaseOrderId = purchaseOrderId;
        PaymentAttemptId = paymentAttemptId;
    }

    public void MarkPaymentSucceeded()
    {
        BusinessException.ThrowIfTrue(
            Status is not AuctionStatus.PaymentPending,
            ErrorMessages.Auction.PaymentNotAvailable,
            ErrorMessages.Exception.CommerceTitle);

        Status = AuctionStatus.Completed;
    }

    public void MarkPaymentFailed()
    {
        BusinessException.ThrowIfTrue(
            Status is not AuctionStatus.PaymentPending,
            ErrorMessages.Auction.PaymentNotAvailable,
            ErrorMessages.Exception.CommerceTitle);

        Status = AuctionStatus.PaymentExpired;
    }

    public void Relist(DateTimeOffset startsAt, DateTimeOffset endsAt)
    {
        BusinessException.ThrowIfTrue(
            Status is not (AuctionStatus.Relistable or AuctionStatus.PaymentExpired or AuctionStatus.Failed),
            ErrorMessages.Auction.InvalidStatusTransition,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            startsAt >= endsAt,
            ErrorMessages.Auction.InvalidDateRange,
            ErrorMessages.Exception.CommerceTitle);

        StartsAt = startsAt;
        EndsAt = endsAt;
        OriginalEndsAt = endsAt;
        CurrentPrice = StartingPrice;
        CurrentWinningBidId = null;
        WinningUserId = null;
        WinningBidAmount = null;
        PurchaseOrderId = null;
        PaymentAttemptId = null;
        WaitingFeeAmount = 0;
        ServiceFeeAmount = 0;
        SellerPayoutAmount = 0;
        PlatformRevenueAmount = 0;
        Status = AuctionStatus.Scheduled;
    }

    private void CalculateSettlement()
    {
        decimal finalAmount = WinningBidAmount ?? 0;
        WaitingFeeAmount = decimal.Round(finalAmount * 0.02m, 2);
        ServiceFeeAmount = decimal.Round(finalAmount * 0.20m, 2);
        PlatformRevenueAmount = WaitingFeeAmount + ServiceFeeAmount;
        SellerPayoutAmount = finalAmount - PlatformRevenueAmount;
    }

    private static void ValidateCore(
        Guid productListingId,
        Guid productId,
        decimal startingPrice,
        decimal minimumBidIncrement,
        DateTimeOffset startsAt,
        DateTimeOffset endsAt,
        int quantity,
        string currency)
    {
        BusinessException.ThrowIfTrue(productListingId == Guid.Empty, ErrorMessages.Auction.ProductListingRequired, ErrorMessages.Exception.CommerceTitle);
        BusinessException.ThrowIfTrue(productId == Guid.Empty, ErrorMessages.Auction.ProductRequired, ErrorMessages.Exception.CommerceTitle);
        BusinessException.ThrowIfTrue(startingPrice < 0, ErrorMessages.Auction.StartingPriceInvalid, ErrorMessages.Exception.CommerceTitle);
        BusinessException.ThrowIfTrue(minimumBidIncrement <= 0, ErrorMessages.Auction.MinimumBidIncrementInvalid, ErrorMessages.Exception.CommerceTitle);
        BusinessException.ThrowIfTrue(startsAt >= endsAt, ErrorMessages.Auction.InvalidDateRange, ErrorMessages.Exception.CommerceTitle);
        BusinessException.ThrowIfTrue(quantity <= 0, ErrorMessages.Auction.QuantityInvalid, ErrorMessages.Exception.CommerceTitle);
        BusinessException.ThrowIfNullOrWhiteSpace(currency, ErrorMessages.Auction.CurrencyRequired, ErrorMessages.Exception.CommerceTitle);
    }
}
