using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Dtos.Auth;
using E_Satis_Auction.Enums;
using E_Satis_Auction.Interfaces;
using E_Satis_Auction.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Satis_Auction.Features.Auth.RefreshToken;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, TokenResponse>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    private const int REFRESH_TOKEN_EXPIRATION_IN_DAYS = 7;

    public RefreshTokenCommandHandler(UserManager<AppUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<TokenResponse> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        AppUser? user = await _userManager.Users
            .FirstOrDefaultAsync(
                u => u.RefreshToken == command.RefreshToken,
                cancellationToken);

        bool isTokenInvalid = user is null || user.RefreshTokenExpiryTime <= DateTime.UtcNow;
        BusinessException.ThrowIfTrue(
            isTokenInvalid,
            ErrorMessages.Auth.InvalidToken,
            ErrorMessages.Exception.TokenTitle);

        BusinessException.ThrowIfTrue(
            user!.UserStatus is UserStatus.Resigned,
            ErrorMessages.Auth.AccountResigned,
            ErrorMessages.Exception.AccountTitle);
        
        BusinessException.ThrowIfTrue(
            user.UserStatus is UserStatus.Suspended,
            ErrorMessages.Auth.AccountSuspended,
            ErrorMessages.Exception.AccountTitle);

        TokenResponse tokenResponse = await _tokenService.GenerateTokenAsync(user);
        user.UpdateRefreshToken(tokenResponse.RefreshToken, DateTime.UtcNow.AddDays(REFRESH_TOKEN_EXPIRATION_IN_DAYS));
        await _userManager.UpdateAsync(user);

        return tokenResponse;
    }
}