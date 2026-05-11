namespace E_Satis_Auction.Dtos.Dispatch.Requests;

public sealed record DispatchItemRequest(
    Guid ItemId,
    int Quantity);