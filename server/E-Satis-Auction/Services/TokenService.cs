using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using E_Satis_Auction.Common.Options;
using E_Satis_Auction.Dtos.Auth;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace E_Satis_Auction.Services;

public class TokenService : ITokenService
{
    private readonly JwtOptions _options;
    private readonly UserManager<AppUser> _userManager;

    public TokenService(IOptions<JwtOptions> options, UserManager<AppUser> userManager)
    {
        _options = options.Value;
        _userManager = userManager;
    }

    public async Task<TokenResponse> GenerateTokenAsync(AppUser user)
    {
        IList<string> userRoles = await _userManager.GetRolesAsync(user);
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];
        claims.AddRange(userRoles.Select(role => new Claim("role", role)));

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_options.SecretKey));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);
        DateTime expiration = DateTime.UtcNow.AddSeconds(_options.ExpirationInSecond);

        JwtSecurityToken token = new(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        string accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        string refreshToken = GenerateRefreshToken();

        return new TokenResponse(accessToken, refreshToken, expiration);
    }

    private static string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[64];
        using RandomNumberGenerator generator = RandomNumberGenerator.Create();
        generator.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }
}