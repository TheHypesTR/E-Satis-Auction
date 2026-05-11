using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Common.Exceptions;

namespace E_Satis_Auction.Models.Dispatches;

public sealed class DispatchLineItem : BaseEntity
{
    public Guid DispatchId { get; private set; }
    public Guid SourceItemId { get; private set; }
    public Guid OriginalItemId { get; private set; }
    public string ItemNameSnapshot { get; private set; }
    public int Quantity { get; private set; }

    private DispatchLineItem()
    {
        ItemNameSnapshot = string.Empty;
    }

    public static DispatchLineItem Create(
        Guid dispatchId,
        Guid sourceItemId,
        Guid originalItemId,
        string itemNameSnapshot,
        int quantity)
    {
        BusinessException.ThrowIfTrue(
            dispatchId == Guid.Empty,
            ErrorMessages.Dispatch.DispatchRequired,
            ErrorMessages.Exception.DispatchTitle);

        BusinessException.ThrowIfTrue(
            sourceItemId == Guid.Empty,
            ErrorMessages.Dispatch.SourceItemRequired,
            ErrorMessages.Exception.DispatchTitle);

        BusinessException.ThrowIfNullOrWhiteSpace(
            itemNameSnapshot,
            ErrorMessages.Dispatch.ItemNameRequired,
            ErrorMessages.Exception.DispatchTitle);

        BusinessException.ThrowIfTrue(
            quantity <= 0,
            ErrorMessages.Dispatch.QuantityMustBePositive,
            ErrorMessages.Exception.DispatchTitle);

        return new DispatchLineItem
        {
            DispatchId = dispatchId,
            SourceItemId = sourceItemId,
            OriginalItemId = originalItemId,
            ItemNameSnapshot = itemNameSnapshot.Trim(),
            Quantity = quantity
        };
    }
}