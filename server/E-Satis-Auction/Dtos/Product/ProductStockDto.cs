using E_Satis_Auction.Dtos.Facility;

namespace E_Satis_Auction.Dtos.Product;

public sealed record ProductStockDto(
    Guid FacilityId,
    string FacilityName,
    FacilityAddressDto Address,
    int TotalAvailableQuantity);