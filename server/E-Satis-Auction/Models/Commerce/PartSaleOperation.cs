using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Models.Commerce;

public sealed class PartSaleOperation : BaseEntity
{
    public Guid SourceItemId { get; private set; }
    public Guid CreatedPartItemId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid FacilityId { get; private set; }
    public int Quantity { get; private set; }
    public UnitOfMeasure UnitOfMeasure { get; private set; }
    public string? Notes { get; private set; }
    public PartSaleOperationStatus Status { get; private set; }

    private PartSaleOperation()
    {
        Status = PartSaleOperationStatus.Completed;
    }

    public static PartSaleOperation Create(Guid sourceItemId, Guid createdPartItemId, Guid productId, Guid facilityId, int quantity, UnitOfMeasure unitOfMeasure, string? notes)
    {
        BusinessException.ThrowIfTrue(sourceItemId == Guid.Empty, ErrorMessages.Dispatch.SourceItemRequired, ErrorMessages.Exception.InventoryTitle);
        BusinessException.ThrowIfTrue(createdPartItemId == Guid.Empty, ErrorMessages.Item.EntityName, ErrorMessages.Exception.InventoryTitle);
        BusinessException.ThrowIfTrue(productId == Guid.Empty, ErrorMessages.Product.EntityName, ErrorMessages.Exception.InventoryTitle);
        BusinessException.ThrowIfTrue(facilityId == Guid.Empty, ErrorMessages.Item.FacilityRequired, ErrorMessages.Exception.InventoryTitle);
        BusinessException.ThrowIfTrue(quantity <= 0, ErrorMessages.PurchaseOrder.QuantityMustBePositive, ErrorMessages.Exception.InventoryTitle);

        return new PartSaleOperation
        {
            SourceItemId = sourceItemId,
            CreatedPartItemId = createdPartItemId,
            ProductId = productId,
            FacilityId = facilityId,
            Quantity = quantity,
            UnitOfMeasure = unitOfMeasure,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            Status = PartSaleOperationStatus.Completed
        };
    }
}
