using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Exceptions;
using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Dtos.Auth;
using e_Sat_Auction.Enums;
using e_Sat_Auction.Interfaces;
using e_Sat_Auction.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace e_Sat_Auction.Features.Auth.Login;

public class LoginCommandHandler : ICommandHandler<LoginCommand, TokenResponse>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    private const int REFRESH_TOKEN_EXPIRATION_IN_DAYS = 7;

    public LoginCommandHandler(UserManager<AppUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<TokenResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        AppUser user = await EnsureUserIsEligibleAsync(command.Identifier);

        await EnsurePasswordIsCorrectAsync(user, command.Password);
        await EnsureEmailIsVerifiedAsync(user);

        return await GenerateTokensAndUpdateUserAsync(user);
    }

    private async Task<AppUser> EnsureUserIsEligibleAsync(string identifier)
    {
        AppUser? user = identifier.Contains('@')
            ? await _userManager.FindByEmailAsync(identifier)
            : await _userManager.Users.SingleOrDefaultAsync(u => u.TCNumber == identifier);

        BusinessException.ThrowIfNull(
            user,
            ErrorMessages.Auth.InvalidCredentials,
            ErrorMessages.Exception.CredentialsTitle);

        BusinessException.ThrowIfTrue(
            user!.UserStatus is UserStatus.Resigned,
            ErrorMessages.Auth.AccountResigned,
            ErrorMessages.Exception.AccountTitle);
        
        BusinessException.ThrowIfTrue(
            user.UserStatus is UserStatus.Suspended,
            ErrorMessages.Auth.AccountSuspended,
            ErrorMessages.Exception.AccountTitle);

        BusinessException.ThrowIfTrue(
            await _userManager.IsLockedOutAsync(user),
            ErrorMessages.Auth.AccountLocked,
            ErrorMessages.Exception.AccountTitle);

        return user;
    }

    private async Task EnsurePasswordIsCorrectAsync(AppUser user, string password)
    {
        bool isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
        if (isPasswordValid)
        {
            await _userManager.ResetAccessFailedCountAsync(user);
            return;
        }

        await _userManager.AccessFailedAsync(user);
        BusinessException.ThrowIfFalse(
            isPasswordValid,
            ErrorMessages.Auth.InvalidCredentials,
            ErrorMessages.Exception.CredentialsTitle);
    }

    private async Task EnsureEmailIsVerifiedAsync(AppUser user)
    {
        bool isEmailVerified = await _userManager.IsEmailConfirmedAsync(user);
        BusinessException.ThrowIfFalse(
            isEmailVerified,
            ErrorMessages.Auth.EmailNotVerified,
            ErrorMessages.Exception.VerificationTitle);
    }

    private async Task<TokenResponse> GenerateTokensAndUpdateUserAsync(AppUser user)
    {
        TokenResponse tokenResponse = await _tokenService.GenerateTokenAsync(user);
        user.UpdateRefreshToken(tokenResponse.RefreshToken, DateTime.UtcNow.AddDays(REFRESH_TOKEN_EXPIRATION_IN_DAYS));

        IdentityResult updateResult = await _userManager.UpdateAsync(user);
        BusinessException.ThrowIfFalse(
            updateResult.Succeeded,
            ErrorMessages.Auth.TokenGenerationFailed,
            ErrorMessages.Exception.TokenTitle);

        return tokenResponse;
    }
}