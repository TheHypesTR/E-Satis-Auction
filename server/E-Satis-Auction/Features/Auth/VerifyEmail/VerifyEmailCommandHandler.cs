using E_Satis_Auction.Extensions;
using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Models.Users;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace E_Satis_Auction.Features.Auth.VerifyEmail;

public class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IDataProtector _dataProtector;

    public VerifyEmailCommandHandler(UserManager<AppUser> userManager, IDataProtectionProvider dataProtectionProvider)
    {
        _userManager = userManager;
        _dataProtector = dataProtectionProvider.CreateProtector(DataProtectionPurposes.EmailVerification);
    }

    public async Task Handle(VerifyEmailCommand command, CancellationToken cancellationToken)
    {
        (string userId, string verificationToken) = _dataProtector.ExtractPayload(
            command.EncryptedPayload,
            ErrorMessages.Validation.InvalidVerificationLink);

        AppUser? user = await _userManager.FindByIdAsync(userId);
        BusinessException.ThrowIfNull(
            user,
            ErrorMessages.Validation.InvalidVerificationLink,
            ErrorMessages.Exception.PayloadTitle);

        if (user!.EmailConfirmed)
        {
            return;
        }

        IdentityResult result = await _userManager.ConfirmEmailAsync(user, verificationToken);
        BusinessException.ThrowIfFalse(
            result.Succeeded,
            ErrorMessages.Validation.InvalidVerificationLink,
            ErrorMessages.Exception.PayloadTitle);
    }
}