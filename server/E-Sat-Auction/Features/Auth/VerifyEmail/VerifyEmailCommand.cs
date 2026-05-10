using e_Sat_Auction.Common.Interfaces.Messaging;

namespace e_Sat_Auction.Features.Auth.VerifyEmail;

public record VerifyEmailCommand(string EncryptedPayload) : ICommand;