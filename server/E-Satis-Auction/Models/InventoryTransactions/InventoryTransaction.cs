using E_Satis_Auction.Common.Entities;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Models.InventoryTransactions;

public sealed class InventoryTransaction : BaseEntity
{
    public Guid ItemId { get; private set; }
    public Guid FacilityId { get; private set; }
    public InventoryTransactionType TransactionType { get; private set; }
    public int QuantityChange { get; private set; }
    public int PreviousQuantity { get; private set; }
    public int NewQuantity { get; private set; }
    public Guid? ReferenceId { get; private set; }
    public string CreatedBy { get; private set; }

    private InventoryTransaction()
    {
        CreatedBy = string.Empty;
    }

    public static InventoryTransaction Create(
        Guid itemId,
        Guid facilityId,
        InventoryTransactionType transactionType,
        int quantityChange,
        int previousQuantity,
        int newQuantity,
        Guid? referenceId,
        string createdBy)
    {
        return new InventoryTransaction
        {
            ItemId = itemId,
            FacilityId = facilityId,
            TransactionType = transactionType,
            QuantityChange = quantityChange,
            PreviousQuantity = previousQuantity,
            NewQuantity = newQuantity,
            ReferenceId = referenceId,
            CreatedBy = createdBy
        };
    }
}