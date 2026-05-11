using E_Satis_Auction.Dtos.Address;
using E_Satis_Auction.Dtos.Manager;

namespace E_Satis_Auction.Dtos.Facility;

public record FacilityDetailDto(
    Guid Id,
    string Name,
    string Description,
    string Status,
    AddressDto? Address,
    List<ManagerDto> Managers);