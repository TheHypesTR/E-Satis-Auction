using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Exceptions;
using e_Sat_Auction.Common.Helpers;
using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Interfaces;
using e_Sat_Auction.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace e_Sat_Auction.Features.User.InviteUser;

public class InviteUserCommandHandler : ICommandHandler<InviteUserCommand, Guid>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserInvitationService _invitationService;
    private readonly ICurrentUserService _currentUserService;

    public InviteUserCommandHandler(
        UserManager<AppUser> userManager,
        IUserInvitationService invitationService,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _invitationService = invitationService;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(InviteUserCommand command, CancellationToken cancellationToken)
    {
        AppUser? inviter = await _userManager.FindByIdAsync(_currentUserService.UserId);
        NotFoundException.ThrowIfNull(inviter, ErrorMessages.User.EntityName, _currentUserService.UserId);
        
        BusinessException.ThrowIfTrue(
            inviter!.Email!.Equals(command.Email, StringComparison.OrdinalIgnoreCase),
            ErrorMessages.User.CannotInviteSelf,
            ErrorMessages.Exception.InvitationTitle);

        IList<string> inviterRoles = await _userManager.GetRolesAsync(inviter);
        bool canAssign = RoleHierarchyHelper.CanAssignRole(inviterRoles, command.TargetRole);
        ForbiddenAccessException.ThrowIfTrue(
            !canAssign,
            ErrorMessages.User.UnauthorizedRoleAssignment,
            ErrorMessages.Exception.InvitationTitle);

        AppUser? existingTargetUser = await _userManager.FindByEmailAsync(command.Email);
        if (existingTargetUser is not null)
        {
            IList<string> targetUserRoles = await _userManager.GetRolesAsync(existingTargetUser);
            bool hasHigherOrEqualRole = RoleHierarchyHelper.HasHigherOrEqualRole(targetUserRoles, command.TargetRole);
            BusinessException.ThrowIfTrue(
                hasHigherOrEqualRole,
                ErrorMessages.User.TargetHasHigherOrEqualRole,
                ErrorMessages.Exception.InvitationTitle);
        }

        AppUser invitedUser = await _invitationService.GetOrAddInvitedUserAsync(
            command.Email,
            command.FirstName,
            command.LastName,
            command.TargetRole);

        return Guid.Parse(invitedUser.Id);
    }
}