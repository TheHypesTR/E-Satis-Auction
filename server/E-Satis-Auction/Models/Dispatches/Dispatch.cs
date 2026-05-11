using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Models.Dispatches;

public sealed class Dispatch : BaseEntity
{
    public string TrackingNumber { get; private set; }
    public Guid SourceFacilityId { get; private set; }
    public Guid? TargetFacilityId { get; private set; }
    public Guid? TargetAddressId { get; private set; }
    public string ReceiverName { get; private set; }
    public string ReceiverPhone { get; private set; }
    public string? Notes { get; private set; }
    public string? DeliveryNote { get; private set; }
    public DispatchStatus Status { get; private set; }
    public DateTimeOffset? DispatchDate { get; private set; }

    private readonly List<DispatchLineItem> _lineItems = [];
    public IReadOnlyCollection<DispatchLineItem> LineItems => _lineItems;

    private Dispatch()
    {
        TrackingNumber = string.Empty;
        ReceiverName = string.Empty;
        ReceiverPhone = string.Empty;
        Status = DispatchStatus.Pending;
    }

    public static Dispatch Create(
        Guid sourceFacilityId,
        Guid? targetFacilityId,
        Guid? targetAddressId,
        string receiverName,
        string receiverPhone,
        string? notes,
        DateTimeOffset? dispatchDate = null,
        DispatchStatus status = DispatchStatus.Pending)
    {
        BusinessException.ThrowIfTrue(
            sourceFacilityId == Guid.Empty,
            ErrorMessages.Dispatch.SourceFacilityRequired,
            ErrorMessages.Exception.DispatchTitle);

        BusinessException.ThrowIfTrue(
            targetFacilityId == Guid.Empty,
            ErrorMessages.Dispatch.TargetFacilityInvalid,
            ErrorMessages.Exception.DispatchTitle);

        BusinessException.ThrowIfTrue(
            targetAddressId == Guid.Empty,
            ErrorMessages.Dispatch.TargetAddressInvalid,
            ErrorMessages.Exception.DispatchTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            receiverName,
            ErrorMessages.Dispatch.ReceiverNameRequired,
            ErrorMessages.Exception.DispatchTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            receiverPhone,
            ErrorMessages.Dispatch.ReceiverPhoneRequired,
            ErrorMessages.Exception.DispatchTitle);

        bool hasFacilityTarget = targetFacilityId.HasValue;
        bool hasAddressTarget = targetAddressId.HasValue;

        BusinessException.ThrowIfTrue(
            hasFacilityTarget == hasAddressTarget,
            ErrorMessages.Dispatch.ExclusiveTargetRequired,
            ErrorMessages.Exception.DispatchTitle);

        return new Dispatch
        {
            TrackingNumber = GenerateTrackingNumber(),
            SourceFacilityId = sourceFacilityId,
            TargetFacilityId = targetFacilityId,
            TargetAddressId = targetAddressId,
            ReceiverName = receiverName.Trim(),
            ReceiverPhone = receiverPhone.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            Status = status,
            DispatchDate = dispatchDate
        };
    }

    public DispatchLineItem AddLineItem(Guid sourceItemId, Guid originalItemId, string itemNameSnapshot, int quantity)
    {
        DispatchLineItem lineItem = DispatchLineItem.Create(Id, sourceItemId, originalItemId, itemNameSnapshot, quantity);
        _lineItems.Add(lineItem);
        
        return lineItem;
    }

    public void MarkInTransit(DateTimeOffset dispatchDate)
    {
        BusinessException.ThrowIfTrue(
            Status is not DispatchStatus.Pending,
            ErrorMessages.Dispatch.StatusNotPending,
            ErrorMessages.Exception.DispatchTitle);

        Status = DispatchStatus.InTransit;
        DispatchDate = dispatchDate;
    }

    public void MarkCompleted(string? deliveryNote = null)
    {
        BusinessException.ThrowIfTrue(
            Status is not DispatchStatus.InTransit,
            ErrorMessages.Dispatch.StatusNotInTransit,
            ErrorMessages.Exception.DispatchTitle);

        if (!string.IsNullOrWhiteSpace(deliveryNote))
        {
            BusinessException.ThrowIfTrue(
                deliveryNote.Length > 1024,
                ErrorMessages.Dispatch.DeliveryNoteMaxLength,
                ErrorMessages.Exception.DispatchTitle);
        }

        DeliveryNote = string.IsNullOrWhiteSpace(deliveryNote) ? null : deliveryNote.Trim();
        Status = DispatchStatus.Completed;
    }

    public void MarkCancelled(string? cancellationNote = null)
    {
        BusinessException.ThrowIfTrue(
            Status is not DispatchStatus.Pending,
            ErrorMessages.Dispatch.StatusNotPending,
            ErrorMessages.Exception.DispatchTitle);
        
        if (!string.IsNullOrWhiteSpace(cancellationNote))
        {
            BusinessException.ThrowIfTrue(
                cancellationNote.Length > 1024,
                ErrorMessages.Dispatch.DeliveryNoteMaxLength,
                ErrorMessages.Exception.DispatchTitle);
        }

        DeliveryNote = string.IsNullOrWhiteSpace(cancellationNote) ? null : cancellationNote.Trim();
        Status = DispatchStatus.Cancelled;
    }

    private static string GenerateTrackingNumber()
    {
        string date = DateTimeOffset.UtcNow.ToString("yyMMdd");
        string part1 = Guid.NewGuid().ToString()[..4].ToUpperInvariant();
        string part2 = Guid.NewGuid().ToString()[..4].ToUpperInvariant();
    
        return $"SVK-{date}-{part1}-{part2}";
    }
}