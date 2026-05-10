namespace e_Sat_Auction.Dtos.Dispatch.Requests;

public sealed record DispatchItemRequest(
    Guid ItemId,
    int Quantity);