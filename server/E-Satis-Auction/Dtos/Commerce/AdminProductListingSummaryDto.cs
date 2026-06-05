using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Commerce;

public sealed record AdminProductListingSummaryDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string Sku,
    Guid SourceFacilityId,
    string SourceFacilityName,
    decimal Price,
    string Currency,
    ProductListingStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    uint Version);
