using E_Satis_Auction.Common.Interfaces.Messaging;

namespace E_Satis_Auction.Features.Auth.VerifyEmail;

public record VerifyEmailCommand(string EncryptedPayload) : ICommand;