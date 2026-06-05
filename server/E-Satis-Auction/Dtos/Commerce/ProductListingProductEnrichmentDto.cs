namespace E_Satis_Auction.Dtos.Commerce;

public sealed record ProductListingProductEnrichmentDto(
    Guid Id,
    string Name,
    string Sku,
    Guid CategoryId,
    bool IsActive);
