using e_Sat_Auction.Common.Interfaces.Messaging;

namespace e_Sat_Auction.Features.Auth.ResendVerificationEmail;

public record ResendVerificationEmailCommand(string Email) : ICommand;