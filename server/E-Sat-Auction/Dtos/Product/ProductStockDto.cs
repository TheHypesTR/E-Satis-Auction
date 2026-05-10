using e_Sat_Auction.Dtos.Facility;

namespace e_Sat_Auction.Dtos.Product;

public sealed record ProductStockDto(
    Guid FacilityId,
    string FacilityName,
    FacilityAddressDto Address,
    int TotalAvailableQuantity);