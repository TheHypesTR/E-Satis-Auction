using E_Satis_Auction.Extensions;
using E_Satis_Auction.Common.Constants;
using E_Satis_Auction.Common.Exceptions;
using E_Satis_Auction.Common.Interfaces.Messaging;
using E_Satis_Auction.Models.Users;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace E_Satis_Auction.Features.User.CompleteInvitation;

public class CompleteInvitationCommandHandler : ICommandHandler<CompleteInvitationCommand>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IDataProtector _dataProtector;

    public CompleteInvitationCommandHandler(UserManager<AppUser> userManager, IDataProtectionProvider dataProtectionProvider)
    {
        _userManager = userManager;
        _dataProtector = dataProtectionProvider.CreateProtector(DataProtectionPurposes.UserInvitation);
    }
    
    public async Task Handle(CompleteInvitationCommand command, CancellationToken cancellationToken)
    {
        (string userId, string resetToken) = _dataProtector.ExtractPayload(
            command.EncryptedPayload,
            ErrorMessages.Validation.InvalidInvitationLink);

        AppUser? user = await _userManager.FindByIdAsync(userId);
        NotFoundException.ThrowIfNull(user, ErrorMessages.User.EntityName, userId);
        BusinessException.ThrowIfTrue(
            user!.UserStatus is not Enums.UserStatus.Invited,
            ErrorMessages.User.NotInvited,
            ErrorMessages.Exception.InvitationTitle);

        IdentityResult passwordResult = await _userManager.ResetPasswordAsync(user, resetToken, command.NewPassword);
        BusinessException.ThrowIfFalse(
            passwordResult.Succeeded,
            ErrorMessages.User.PasswordSetFailed,
            ErrorMessages.Exception.PayloadTitle);

        user.CompleteInvitation(
            command.FirstName,
            command.LastName,
            command.TCNumber,
            command.PhoneNumber,
            command.BirthDate,
            command.Gender);

        IdentityResult updateResult = await _userManager.UpdateAsync(user);
        BusinessException.ThrowIfFalse(
            updateResult.Succeeded,
            ErrorMessages.User.UpdateFailed,
            ErrorMessages.Exception.AccountTitle);
    }
}