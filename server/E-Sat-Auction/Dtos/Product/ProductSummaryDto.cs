using e_Sat_Auction.Enums;

namespace e_Sat_Auction.Dtos.Product;

public sealed record ProductSummaryDto(
    Guid Id,
    string Sku,
    string? Barcode,
    string Name,
    string CategoryName,
    UnitOfMeasure UnitOfMeasure,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    uint Version);