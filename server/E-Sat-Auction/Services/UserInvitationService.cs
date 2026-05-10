using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Exceptions;
using e_Sat_Auction.Common.Helpers;
using e_Sat_Auction.Common.Interfaces;
using e_Sat_Auction.Common.Options;
using e_Sat_Auction.Enums;
using e_Sat_Auction.Interfaces;
using e_Sat_Auction.Models.Users;
using e_Sat_Auction.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace e_Sat_Auction.Services;

public class UserInvitationService : IUserInvitationService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDataProtector _dataProtector;
    private readonly ClientOptions _clientOptions;
    
    private const string COMPLETE_INVITE = "complete-invite";
    private const string COMPLETE_INVITE_PAYLOAD = "payload";

    public UserInvitationService(
        UserManager<AppUser> userManager,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        IDataProtectionProvider dataProtectionProvider,
        IOptions<ClientOptions> clientOptions)
    {
        _userManager = userManager;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _dataProtector = dataProtectionProvider.CreateProtector(DataProtectionPurposes.UserInvitation);
        _clientOptions = clientOptions.Value;
    }

    public async Task<AppUser> GetOrAddInvitedUserAsync(string email, string firstName, string lastName, string targetRole)
    {
        AppUser? existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            await AssignRoleIfNotExistsAsync(existingUser, targetRole);
            return existingUser;
        }

        return await AddShadowUserAndSendInviteAsync(email, firstName, lastName, targetRole);
    }

    private async Task<AppUser> AddShadowUserAndSendInviteAsync(string email, string firstName, string lastName, string targetRole)
    {
        AppUser invitedUser = AppUser.AddInvited(firstName, lastName, email);
        string tempPassword = $"Temp.{Guid.NewGuid().ToString()[..8]}!A";

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            IdentityResult result = await _userManager.CreateAsync(invitedUser, tempPassword);
            BusinessException.ThrowIfFalse(
                result.Succeeded,
                ErrorMessages.User.InvitationFailed,
                ErrorMessages.Exception.InvitationTitle);

            await AssignRoleIfNotExistsAsync(invitedUser, targetRole);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        await GenerateTokenAndSendEmailAsync(invitedUser);
        return invitedUser;
    }

    private async Task AssignRoleIfNotExistsAsync(AppUser user, string targetRole)
    {
        BusinessException.ThrowIfTrue(
            user.UserStatus is UserStatus.Resigned,
            ErrorMessages.Auth.AccountResigned,
            ErrorMessages.Exception.RoleAssignmentTitle);
        
        BusinessException.ThrowIfTrue(
            user.UserStatus is UserStatus.Suspended,
            ErrorMessages.Auth.AccountSuspended,
            ErrorMessages.Exception.RoleAssignmentTitle);
        
        IList<string> currentRoles = await _userManager.GetRolesAsync(user);
        if (RoleHierarchyHelper.HasHigherOrEqualRole(currentRoles, targetRole))
        {
            return;
        }

        if (currentRoles.Any())
        {
            IdentityResult removeRoleResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            BusinessException.ThrowIfFalse(
                removeRoleResult.Succeeded,
                ErrorMessages.User.UpdateFailed,
                ErrorMessages.Exception.RoleAssignmentTitle);
        }

        IdentityResult addRoleResult = await _userManager.AddToRoleAsync(user, targetRole);
        BusinessException.ThrowIfFalse(
            addRoleResult.Succeeded,
            ErrorMessages.User.UpdateFailed,
            ErrorMessages.Exception.RoleAssignmentTitle);
    }

    private async Task GenerateTokenAndSendEmailAsync(AppUser user)
    {
        string token = await _userManager.GeneratePasswordResetTokenAsync(user);
        string urlEncodedPayload = _dataProtector.GenerateUrlEncodedPayload(user.Id, token);

        string baseUrl = _clientOptions.Url.TrimEnd('/');
        string invitationLink = $"{baseUrl}/{COMPLETE_INVITE}?{COMPLETE_INVITE_PAYLOAD}={urlEncodedPayload}";
        await _emailService.SendInvitationEmailAsync(user.Email!, user.FirstName, invitationLink);
    }
}