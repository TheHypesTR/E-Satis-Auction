using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Features.Auth.Register;

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