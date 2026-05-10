using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.Auth;

namespace e_Sat_Auction.Features.Auth.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : ICommand<TokenResponse>;