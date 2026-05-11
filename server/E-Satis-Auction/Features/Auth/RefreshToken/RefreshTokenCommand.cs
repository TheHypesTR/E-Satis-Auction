using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Auth;

namespace E_Satis_Auction.Features.Auth.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : ICommand<TokenResponse>;