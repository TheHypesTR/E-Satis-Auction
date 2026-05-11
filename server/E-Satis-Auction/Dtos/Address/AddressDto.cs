namespace E_Satis_Auction.Dtos.Address;

public record AddressDto(
    string Title,
    string City,
    string District,
    string OpenAddress,
    double Latitude,
    double Longitude,
    bool IsTemporary);