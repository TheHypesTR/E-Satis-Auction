namespace E_Satis_Auction.Dtos.Commerce.Requests;

public sealed record ReceiveReturnRequestRequest(
    string? Note,
    Guid? TargetFacilityId,
    IReadOnlyCollection<ReceiveReturnRequestLineRequest>? Lines);
