using e_Sat_Auction.Dtos.Auth;
using e_Sat_Auction.Models.Users;

namespace e_Sat_Auction.Interfaces;

public interface ITokenService
{
    Task<TokenResponse> GenerateTokenAsync(AppUser user);
}