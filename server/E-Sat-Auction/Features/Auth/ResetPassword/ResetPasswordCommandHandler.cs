using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Exceptions;
using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Interfaces;
using e_Sat_Auction.Models.Users;
using e_Sat_Auction.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace e_Sat_Auction.Features.Auth.ResetPassword;

public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IDataProtector _dataProtector;

    private const string RESET_PASSWORD_PROTECTOR = "PasswordReset";

    public ResetPasswordCommandHandler(
        UserManager<AppUser> userManager,
        IEmailService emailService,
        IDataProtectionProvider dataProtectionProvider)
    {
        _userManager = userManager;
        _emailService = emailService;
        _dataProtector = dataProtectionProvider.CreateProtector(RESET_PASSWORD_PROTECTOR);
    }

    public async Task Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        (string userId, string resetToken) = _dataProtector.ExtractPayload(
            command.EncryptedPayload,
            ErrorMessages.Validation.InvalidResetLink);

        AppUser? user = await _userManager.FindByIdAsync(userId);
        BusinessException.ThrowIfNull(
            user,
            ErrorMessages.Validation.InvalidResetLink,
            ErrorMessages.Exception.PayloadTitle);

        IdentityResult result = await _userManager.ResetPasswordAsync(user!, resetToken, command.NewPassword);
        BusinessException.ThrowIfFalse(
            result.Succeeded,
            ErrorMessages.Validation.InvalidResetLink,
            ErrorMessages.Exception.PayloadTitle);

        await _emailService.SendPasswordChangedEmailAsync(user!.Email!, user.FirstName);
    }
}