using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Interfaces;
using e_Sat_Auction.Resources;
using Microsoft.Extensions.Localization;

namespace e_Sat_Auction.Services;

public class EmailTemplateService : IEmailTemplateService
{
    private readonly IStringLocalizer<SharedResource> _localizer;

    private const string EMAIL_TEMPLATE = "BaseEmailTemplate.html";

    public EmailTemplateService(IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer;
    }

    public async Task<string> GenerateEmailVerificationEmailAsync(string firstName, string verificationLink)
    {
        string mailBody = await GetBaseTemplateAsync(firstName);

        return mailBody
            .Replace("{{BodyText}}", _localizer[EmailMessages.VerifyEmailBody].Value)
            .Replace("{{ButtonDisplay}}", "block")
            .Replace("{{ActionLink}}", verificationLink)
            .Replace("{{ButtonText}}", _localizer[EmailMessages.VerifyEmailButton].Value)
            .Replace("{{FallbackText}}", _localizer[EmailMessages.FallbackText].Value)
            .Replace("{{SecurityNotice}}", _localizer[EmailMessages.VerifyEmailNotice].Value);
    }

    public async Task<string> GenerateInvitationEmailAsync(string firstName, string invitationLink)
    {
        string mailBody = await GetBaseTemplateAsync(firstName);

        return mailBody
            .Replace("{{BodyText}}", _localizer[EmailMessages.InviteBody].Value)
            .Replace("{{ButtonDisplay}}", "block")
            .Replace("{{ActionLink}}", invitationLink)
            .Replace("{{ButtonText}}", _localizer[EmailMessages.InviteButton].Value)
            .Replace("{{FallbackText}}", _localizer[EmailMessages.FallbackText].Value)
            .Replace("{{SecurityNotice}}", _localizer[EmailMessages.InviteNotice].Value);
    }

    public async Task<string> GeneratePasswordResetEmailAsync(string firstName, string resetLink)
    {
        string mailBody = await GetBaseTemplateAsync(firstName);

        return mailBody
            .Replace("{{BodyText}}", _localizer[EmailMessages.ResetPasswordBody].Value)
            .Replace("{{ButtonDisplay}}", "block")
            .Replace("{{ActionLink}}", resetLink)
            .Replace("{{ButtonText}}", _localizer[EmailMessages.ResetPasswordButton].Value)
            .Replace("{{FallbackText}}", _localizer[EmailMessages.FallbackText].Value)
            .Replace("{{SecurityNotice}}", _localizer[EmailMessages.SecurityNotice].Value);
    }

    public async Task<string> GeneratePasswordChangedEmailAsync(string firstName)
    {
        string mailBody = await GetBaseTemplateAsync(firstName);

        return mailBody
            .Replace("{{BodyText}}", _localizer[EmailMessages.PasswordChangedBody].Value)
            .Replace("{{ButtonDisplay}}", "none")
            .Replace("{{ActionLink}}", "#")
            .Replace("{{FallbackText}}", "")
            .Replace("{{SecurityNotice}}", _localizer[EmailMessages.PasswordChangedNotice].Value);
    }

    public async Task<string> GenerateFacilityAssignedEmailAsync(string firstName, string facilityName, string loginLink)
    {
        string mailBody = await GetBaseTemplateAsync(firstName);

        return mailBody
            .Replace("{{BodyText}}", _localizer[EmailMessages.FacilityAssignedBody].Value
                .Replace("{{FacilityName}}", facilityName))
            .Replace("{{ButtonDisplay}}", "block")
            .Replace("{{ActionLink}}", loginLink)
            .Replace("{{ButtonText}}", _localizer[EmailMessages.FacilityAssignedButton].Value)
            .Replace("{{FallbackText}}", _localizer[EmailMessages.FallbackText].Value)
            .Replace("{{SecurityNotice}}", _localizer[EmailMessages.FacilityAssignedNotice].Value);
    }

    private async Task<string> GetBaseTemplateAsync(string firstName)
    {
        string templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", EMAIL_TEMPLATE);
        string mailBody = await File.ReadAllTextAsync(templatePath);

        return mailBody
            .Replace("{{PlatformName}}", "Emergency Platform HUB")
            .Replace("{{Greeting}}", _localizer[EmailMessages.Greeting].Value)
            .Replace("{{FirstName}}", firstName)
            .Replace("{{AllRightsReserved}}", _localizer[EmailMessages.RightsReserved].Value);
    }
}