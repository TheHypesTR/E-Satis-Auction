using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Dtos.Commerce;

public sealed record ProductListingSummaryDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string Sku,
    Guid SourceFacilityId,
    string SourceFacilityName,
    decimal Price,
    string Currency,
    ProductListingStatus Status,
    DateTimeOffset? ActiveFrom,
    DateTimeOffset? ActiveUntil,
    uint Version);
