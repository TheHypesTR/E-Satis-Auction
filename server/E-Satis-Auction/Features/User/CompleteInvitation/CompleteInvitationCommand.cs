using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Enums;

namespace E_Satis_Auction.Features.User.CompleteInvitation;

public record CompleteInvitationCommand(
    string EncryptedPayload,
    string FirstName,
    string LastName,
    string NewPassword,
    string ConfirmPassword,
    string? TCNumber,
    string PhoneNumber,
    DateTime BirthDate,
    Gender Gender) : ICommand;