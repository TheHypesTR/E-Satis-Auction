using E_Satis_Auction.Common.Interfaces.Messaging;

namespace E_Satis_Auction.Features.Auth.ForgotPassword;

public record ForgotPasswordCommand(string Email) : ICommand;