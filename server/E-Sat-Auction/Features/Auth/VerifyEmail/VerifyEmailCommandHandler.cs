using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Exceptions;
using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Models.Users;
using e_Sat_Auction.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace e_Sat_Auction.Features.Auth.VerifyEmail;

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