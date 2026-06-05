namespace E_Satis_Auction.Dtos.Commerce.Requests;

public sealed record CreateReturnRequestLineRequest(Guid PurchaseOrderLineId, int Quantity, string? Reason);
