using e_Sat_Auction.Enums;

namespace e_Sat_Auction.Dtos.Dispatch.Requests;

public sealed record ReceiveDispatchLineItemRequest(
    Guid SourceItemId,
    ItemMode Mode,
    Guid? MappedProductId,
    int ReceivedQuantity,
    int DamagedQuantity);