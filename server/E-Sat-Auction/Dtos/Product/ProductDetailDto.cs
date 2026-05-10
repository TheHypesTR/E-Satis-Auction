using e_Sat_Auction.Enums;

namespace e_Sat_Auction.Dtos.Product;

public sealed record ProductDetailDto(
    Guid Id,
    string Sku,
    string? Barcode,
    string Name,
    Guid CategoryId,
    string CategoryName,
    UnitOfMeasure UnitOfMeasure,
    IReadOnlyDictionary<string, string> BaseAttributes,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    uint Version,
    List<ProductStockDto> FacilityStocks);