using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Auth;

namespace E_Satis_Auction.Features.Auth.Login;

public record LoginCommand(string Identifier, string Password) : ICommand<TokenResponse>;