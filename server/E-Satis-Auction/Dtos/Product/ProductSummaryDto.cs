using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Product;

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