namespace E_Satis_Auction.Dtos.Manager.Requests;

public record AssignManagerRequest(
    string Email,
    string FirstName,
    string LastName,
    bool IsPrimary = false);