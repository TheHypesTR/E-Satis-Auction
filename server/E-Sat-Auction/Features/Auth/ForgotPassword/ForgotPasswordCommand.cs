using e_Sat_Auction.Common.Interfaces.Messaging;

namespace e_Sat_Auction.Features.Auth.ForgotPassword;

public record ForgotPasswordCommand(string Email) : ICommand;