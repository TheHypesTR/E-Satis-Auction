using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.Auth;

namespace e_Sat_Auction.Features.Auth.Login;

public record LoginCommand(string Identifier, string Password) : ICommand<TokenResponse>;