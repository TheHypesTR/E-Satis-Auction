using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Enums;

namespace e_Sat_Auction.Features.User.CompleteInvitation;

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