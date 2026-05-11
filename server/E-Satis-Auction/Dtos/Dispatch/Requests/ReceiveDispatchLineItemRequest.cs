using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Dispatch.Requests;

public sealed record ReceiveDispatchLineItemRequest(
    Guid SourceItemId,
    ItemMode Mode,
    Guid? MappedProductId,
    int ReceivedQuantity,
    int DamagedQuantity);