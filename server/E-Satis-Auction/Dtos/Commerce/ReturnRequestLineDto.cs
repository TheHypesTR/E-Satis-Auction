namespace E_Satis_Auction.Dtos.Commerce;

public sealed record ReturnRequestLineDto(
    Guid Id,
    Guid PurchaseOrderLineId,
    int Quantity,
    string? Reason,
    int ReceivedQuantity,
    int RestockedQuantity,
    string? ReceiveNote);
