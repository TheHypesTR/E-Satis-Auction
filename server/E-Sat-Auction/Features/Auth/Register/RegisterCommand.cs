using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Enums;

namespace e_Sat_Auction.Features.Auth.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,
    string ConfirmPassword,
    string? TCNumber,
    Gender? Gender,
    DateTime? BirthDate) : ICommand<Guid>;