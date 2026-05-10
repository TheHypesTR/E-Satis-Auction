using e_Sat_Auction.Enums;

namespace e_Sat_Auction.Dtos.Item;

public sealed record ItemSummaryDto(
    Guid Id,
    string DisplayName,
    ItemMode Mode,
    ItemStatus Status,
    int Quantity,
    UnitOfMeasure UnitOfMeasure,
    Guid FacilityId,
    string FacilityName,
    Guid CategoryId,
    string CategoryName,
    Guid? ProductId,
    string? ProductName,
    DateTime CreatedAt,
    DateTime UpdatedAt);