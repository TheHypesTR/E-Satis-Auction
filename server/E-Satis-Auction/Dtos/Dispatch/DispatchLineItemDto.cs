namespace E_Satis_Auction.Dtos.Dispatch;

public sealed record DispatchLineItemDto(
    Guid SourceItemId,
    string ItemNameSnapshot,
    int Quantity);