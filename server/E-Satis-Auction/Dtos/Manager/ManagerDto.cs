namespace E_Satis_Auction.Dtos.Manager;

public record ManagerDto(
    string UserId,
    string FirstName,
    string LastName,
    string Email,
    bool IsPrimary);