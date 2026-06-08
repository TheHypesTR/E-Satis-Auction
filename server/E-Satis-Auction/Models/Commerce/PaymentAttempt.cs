using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Models.Commerce;

public sealed class PaymentAttempt : BaseEntity
{
    public Guid PurchaseOrderId { get; private set; }
    public string UserId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public string IdempotencyKey { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? FailureReason { get; private set; }
    public uint Version { get; private set; }

    private PaymentAttempt()
    {
        UserId = string.Empty;
        Currency = string.Empty;
        IdempotencyKey = string.Empty;
        Status = PaymentStatus.Created;
    }

    public static PaymentAttempt Create(
        Guid purchaseOrderId,
        string userId,
        decimal amount,
        string currency,
        string idempotencyKey,
        DateTimeOffset expiresAt)
    {
        BusinessException.ThrowIfTrue(
            purchaseOrderId == Guid.Empty,
            ErrorMessages.PurchaseOrder.EntityName,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            userId,
            ErrorMessages.PurchaseOrder.UserRequired,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            amount < 0,
            ErrorMessages.Payment.AmountInvalid,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            currency,
            ErrorMessages.PurchaseOrder.CurrencyRequired,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            idempotencyKey,
            ErrorMessages.Payment.IdempotencyKeyRequired,
            ErrorMessages.Exception.CommerceTitle);

        return new PaymentAttempt
        {
            PurchaseOrderId = purchaseOrderId,
            UserId = userId,
            Amount = amount,
            Currency = currency.Trim().ToUpperInvariant(),
            IdempotencyKey = idempotencyKey.Trim(),
            ExpiresAt = expiresAt,
            Status = PaymentStatus.Created
        };
    }

    public void EnterPaymentEntry()
    {
        EnsureStatus(PaymentStatus.Created);
        Status = PaymentStatus.PaymentEntry;
    }

    public void MarkProcessing()
    {
        EnsureStatus(PaymentStatus.PaymentEntry);
        Status = PaymentStatus.Processing;
    }

    public void MarkSucceeded()
    {
        BusinessException.ThrowIfTrue(
            Status is not (PaymentStatus.PaymentEntry or PaymentStatus.Processing),
            ErrorMessages.Payment.InvalidStateTransition,
            ErrorMessages.Exception.CommerceTitle);

        Status = PaymentStatus.Succeeded;
    }

    public void MarkFailed(string reason)
    {
        BusinessException.ThrowIfTrue(
            Status is PaymentStatus.Succeeded or PaymentStatus.Failed or PaymentStatus.Expired or PaymentStatus.Cancelled,
            ErrorMessages.Payment.InvalidStateTransition,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            reason,
            ErrorMessages.Payment.FailureReasonRequired,
            ErrorMessages.Exception.CommerceTitle);

        Status = PaymentStatus.Failed;
        FailureReason = reason.Trim();
    }

    public void MarkExpired(DateTimeOffset now)
    {
        BusinessException.ThrowIfTrue(
            Status is PaymentStatus.Succeeded or PaymentStatus.Failed or PaymentStatus.Expired or PaymentStatus.Cancelled,
            ErrorMessages.Payment.InvalidStateTransition,
            ErrorMessages.Exception.CommerceTitle);

        BusinessException.ThrowIfTrue(
            ExpiresAt > now,
            ErrorMessages.Payment.NotExpired,
            ErrorMessages.Exception.CommerceTitle);

        Status = PaymentStatus.Expired;
    }

    public bool IsActiveReservationExpired(DateTimeOffset now)
    {
        return Status is PaymentStatus.PaymentEntry or PaymentStatus.Processing && ExpiresAt <= now;
    }

    private void EnsureStatus(PaymentStatus expected)
    {
        BusinessException.ThrowIfTrue(
            Status != expected,
            ErrorMessages.Payment.InvalidStateTransition,
            ErrorMessages.Exception.CommerceTitle);
    }
}
