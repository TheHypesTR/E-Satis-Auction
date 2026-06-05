namespace E_Satis_Auction.Dtos.Commerce;

public sealed record OrderLineAllocationDto(Guid Id, Guid OriginalItemId, Guid ReservedItemId, int Quantity);
