using e_Sat_Auction.Common;
using e_Sat_Auction.Common.Attributes;
using e_Sat_Auction.Common.Controllers;
using e_Sat_Auction.Dtos.User;
using e_Sat_Auction.Features.User.CompleteInvitation;
using e_Sat_Auction.Features.User.GetUserById;
using e_Sat_Auction.Features.User.InviteUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace e_Sat_Auction.Controllers;

public class UserController : AuthorizedBaseController
{
    /// <summary>
    /// Retrieves a user by unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user</param>
    /// <returns>User details</returns>
    /// <response code="200">User found and returned.</response>
    /// <response code="404">User with the specified ID does not exist.</response>
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireRoles(AppRoles.GeneralAdmin, AppRoles.NGOAdmin, AppRoles.WarehouseManager)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        UserDto result = await Mediator.Send(new GetUserByIdQuery(id));

        return Ok(result);
    }
    
    /// <summary>
    /// Invites a new user to the system and add a shadow account (Invited status).
    /// </summary>
    /// <param name="request">The basic information and target role of the user to be invited</param>
    /// <returns>Returns the ID of the invited user</returns>
    /// <response code="201">Invitation was successfully sent.</response>
    /// <response code="400">If validation fails or the inviter lacks sufficient role permissions.</response>
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [RequireRoles(AppRoles.GeneralAdmin, AppRoles.NGOAdmin, AppRoles.WarehouseManager)]
    [HttpPost("invite")]
    public async Task<IActionResult> InviteUser([FromBody] InviteUserCommand request)
    {
        Guid userId = await Mediator.Send(request);

        return CreatedAtAction(nameof(GetUserById), new { id = userId }, new { id = userId });
    }

    /// <summary>
    /// Completes the profile of an invited user and activates their account.
    /// </summary>
    /// <param name="command">Encrypted payload, new password, and missing profile details</param>
    /// <response code="204">Profile completed and account successfully activated.</response>
    /// <response code="400">If the link is invalid, expired, or validation rules fail.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    [HttpPost("complete-invite")]
    public async Task<IActionResult> CompleteInvitation([FromBody] CompleteInvitationCommand command)
    {
        await Mediator.Send(command);

        return NoContent();
    }
}