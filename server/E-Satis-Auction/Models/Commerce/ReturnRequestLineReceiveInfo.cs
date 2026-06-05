namespace E_Satis_Auction.Models.Commerce;

public sealed record ReturnRequestLineReceiveInfo(
    Guid ReturnRequestLineId,
    int ReceivedQuantity,
    int RestockedQuantity,
    string? Note);
