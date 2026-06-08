using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Commerce;

public sealed record PartSaleOperationDto(
    Guid Id,
    Guid SourceItemId,
    Guid CreatedPartItemId,
    Guid ProductId,
    Guid FacilityId,
    int Quantity,
    UnitOfMeasure UnitOfMeasure,
    string? Notes,
    PartSaleOperationStatus Status,
    DateTime CreatedAt);
