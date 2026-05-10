using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Interfaces.Messaging;
using e_Sat_Auction.Common.Options;
using e_Sat_Auction.Interfaces;
using e_Sat_Auction.Models.Users;
using e_Sat_Auction.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace e_Sat_Auction.Features.Auth.ForgotPassword;

public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IDataProtector _dataProtector;
    private readonly ClientOptions _clientOptions;

    public ForgotPasswordCommandHandler(
        UserManager<AppUser> userManager,
        IEmailService emailService,
        IDataProtectionProvider dataProtectionProvider,
        IOptions<ClientOptions> clientOptions)
    {
        _userManager = userManager;
        _emailService = emailService;
        _dataProtector = dataProtectionProvider.CreateProtector(DataProtectionPurposes.PasswordReset);
        _clientOptions = clientOptions.Value;
    }

    public async Task Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        AppUser? user = await _userManager.FindByEmailAsync(command.Email);
        if (user is null)
        {
            int fakeDelay = Random.Shared.Next(1500, 3000);
            await Task.Delay(fakeDelay, cancellationToken);
            return;
        }

        string token = await _userManager.GeneratePasswordResetTokenAsync(user);
        string urlEncodedPayload = _dataProtector.GenerateUrlEncodedPayload(user.Id, token);
        
        string baseUrl = _clientOptions.Url.TrimEnd('/');
        string frontendResetLink = $"{baseUrl}/reset-password?payload={urlEncodedPayload}";

        await _emailService.SendPasswordResetEmailAsync(user.Email!, user.FirstName, frontendResetLink);
    }
}