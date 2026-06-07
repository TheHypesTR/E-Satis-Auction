using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Commerce.Requests;

public sealed record CreatePartSaleOperationRequest(
    Guid SourceItemId,
    Guid ProductId,
    int Quantity,
    Guid FacilityId,
    UnitOfMeasure UnitOfMeasure,
    Dictionary<string, string>? DynamicAttributes,
    string? Notes);
