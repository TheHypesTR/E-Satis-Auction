using e_Sat_Auction.Dtos.Address;
using e_Sat_Auction.Dtos.Manager;

namespace e_Sat_Auction.Dtos.Facility;

public record FacilityDetailDto(
    Guid Id,
    string Name,
    string Description,
    string Status,
    AddressDto? Address,
    List<ManagerDto> Managers);