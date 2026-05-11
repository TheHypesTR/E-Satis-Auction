using E_Satis_Auction.Dtos.Auth;
using E_Satis_Auction.Models.Users;

namespace E_Satis_Auction.Interfaces;

public interface ITokenService
{
    Task<TokenResponse> GenerateTokenAsync(AppUser user);
}