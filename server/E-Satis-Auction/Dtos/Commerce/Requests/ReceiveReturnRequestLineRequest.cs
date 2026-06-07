namespace E_Satis_Auction.Dtos.Commerce.Requests;

public sealed record ReceiveReturnRequestLineRequest(
    Guid ReturnRequestLineId,
    int ReceivedQuantity,
    int RestockQuantity,
    string? Note);
