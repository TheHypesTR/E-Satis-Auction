namespace e_Sat_Auction.Dtos.Dispatch.Requests;

public sealed record CreateDispatchRequest(
    Guid? TargetFacilityId,
    Guid? TargetAddressId,
    string ReceiverName,
    string ReceiverPhone,
    string? Notes,
    List<DispatchItemRequest> Items);