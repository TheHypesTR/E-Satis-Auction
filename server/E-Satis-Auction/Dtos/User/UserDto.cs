namespace E_Satis_Auction.Dtos.User;

public record UserDto(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string? TCNumber,
    IList<string> Roles,
    string UserStatus,
    string Gender,
    DateTime BirthDate);