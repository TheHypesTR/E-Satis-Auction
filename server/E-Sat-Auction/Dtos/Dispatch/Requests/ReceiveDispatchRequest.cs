namespace e_Sat_Auction.Dtos.Dispatch.Requests;

public sealed record ReceiveDispatchRequest(
    List<ReceiveDispatchLineItemRequest> Items,
    string? DeliveryNote);