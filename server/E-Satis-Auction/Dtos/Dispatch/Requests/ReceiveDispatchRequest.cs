namespace E_Satis_Auction.Dtos.Dispatch.Requests;

public sealed record ReceiveDispatchRequest(
    List<ReceiveDispatchLineItemRequest> Items,
    string? DeliveryNote);