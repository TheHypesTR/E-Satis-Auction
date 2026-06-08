using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Commerce;

public sealed record ProductListingDetailDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string Sku,
    Guid CategoryId,
    Guid SourceFacilityId,
    string SourceFacilityName,
    decimal Price,
    string Currency,
    ProductListingStatus Status,
    int AvailableStockQuantity,
    DateTimeOffset? ActiveFrom,
    DateTimeOffset? ActiveUntil,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    uint Version);
