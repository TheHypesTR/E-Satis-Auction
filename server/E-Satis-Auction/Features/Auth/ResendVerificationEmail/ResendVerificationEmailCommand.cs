using E_Satis_Auction.Common.Interfaces.Messaging;

namespace E_Satis_Auction.Features.Auth.ResendVerificationEmail;

public record ResendVerificationEmailCommand(string Email) : ICommand;