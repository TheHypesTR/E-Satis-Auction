using e_Sat_Auction.Common.Controllers;
using e_Sat_Auction.Dtos.Auth;
using e_Sat_Auction.Dtos.User;
using e_Sat_Auction.Extensions;
using e_Sat_Auction.Features.Auth.ForgotPassword;
using e_Sat_Auction.Features.Auth.GetMe;
using e_Sat_Auction.Features.Auth.Login;
using e_Sat_Auction.Features.Auth.RefreshToken;
using e_Sat_Auction.Features.Auth.Register;
using e_Sat_Auction.Features.Auth.ResendVerificationEmail;
using e_Sat_Auction.Features.Auth.ResetPassword;
using e_Sat_Auction.Features.Auth.VerifyEmail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace e_Sat_Auction.Controllers;

public class AuthController : BaseController
{
    /// <summary>
    /// Retrieves the profile information of the currently authenticated user.
    /// </summary>
    /// <returns>User profile details without sensitive information</returns>
    /// <response code="200">Returns the user details.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="404">If the user is authenticated but not found in the database.</response>
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        UserDto userDto = await Mediator.Send(new GetMeQuery());

        return Ok(userDto);
    }

    /// <summary>
    /// Generates a new Access Token using a valid Refresh Token.
    /// </summary>
    /// <param name="request">The refresh token string</param>
    /// <returns>New Access Token and Refresh Token details</returns>
    /// <response code="200">Returns the new token details.</response>
    /// <response code="400">If the refresh token is invalid or expired.</response>
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand request)
    {
        TokenResponse response = await Mediator.Send(request);

        return Ok(response);
    }

    /// <summary>
    /// Registers a new user to the system.
    /// </summary>
    /// <param name="request">User information required for registration</param>
    /// <returns>Returns the ID of the created user</returns>
    /// <response code="201">User was successfully created.</response>
    /// <response code="400">If the submitted model is invalid, validation rules are not met, or the Email/TC is already registered.</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [EnableRateLimiting(RateLimitingExtension.STRICT_AUTH_POLICY)]
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand request)
    {
        Guid userId = await Mediator.Send(request);

        return CreatedAtAction(nameof(GetMe), new { id = userId }, new { id = userId });
    }

    /// <summary>
    /// Authenticates a user and returns a JWT along with a Refresh Token.
    /// </summary>
    /// <param name="request">User login credentials (Email and Password)</param>
    /// <returns>Access Token and Refresh Token details</returns>
    /// <response code="200">Returns the token details.</response>
    /// <response code="400">If the credentials are invalid or the account is resigned/suspended.</response>
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [EnableRateLimiting(RateLimitingExtension.STRICT_AUTH_POLICY)]
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand request)
    {
        TokenResponse response = await Mediator.Send(request);

        return Ok(response);
    }

    /// <summary>
    /// Verifies the user's email address using the encrypted payload sent to their email.
    /// </summary>
    /// <param name="request">Encrypted payload containing User ID and Verification Token</param>
    /// <response code="204">Email verified successfully.</response>
    /// <response code="400">If the payload is invalid or expired.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [EnableRateLimiting(RateLimitingExtension.STRICT_AUTH_POLICY)]
    [AllowAnonymous]
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand request)
    {
        await Mediator.Send(request);

        return NoContent();
    }

    /// <summary>
    /// Resends the email verification link if the user exists and is not yet verified.
    /// </summary>
    /// <param name="request">User's email address</param>
    /// <response code="204">Returns a success message regardless of whether the email exists or is already verified (Security Best Practice).</response>
    /// <response code="400">If the submitted email format is invalid.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [EnableRateLimiting(RateLimitingExtension.STRICT_AUTH_POLICY)]
    [AllowAnonymous]
    [HttpPost("resend-verification-email")]
    public async Task<IActionResult> ResendVerificationEmail([FromBody] ResendVerificationEmailCommand request)
    {
        await Mediator.Send(request);

        return NoContent();
    }

    /// <summary>
    /// Sends a password reset link to the user's email address if it exists in the system.
    /// </summary>
    /// <param name="request">User's email address</param>
    /// <response code="204">Returns a success message regardless of whether the email exists or not (Security Best Practice).</response>
    /// <response code="400">If the submitted email format is invalid.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [EnableRateLimiting(RateLimitingExtension.STRICT_AUTH_POLICY)]
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand request)
    {
        await Mediator.Send(request);

        return NoContent();
    }

    /// <summary>
    /// Resets the user's password using the provided token and new password.
    /// </summary>
    /// <param name="request">Email, Token, and New Password details</param>
    /// <response code="204">Password has been successfully reset.</response>
    /// <response code="400">If the token is invalid, expired, or validation rules fail.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [EnableRateLimiting(RateLimitingExtension.STRICT_AUTH_POLICY)]
    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand request)
    {
        await Mediator.Send(request);

        return NoContent();
    }
}