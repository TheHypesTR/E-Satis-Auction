using e_Sat_Auction.Common.Constants;
using e_Sat_Auction.Common.Options;
using e_Sat_Auction.Interfaces;
using e_Sat_Auction.Resources;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace e_Sat_Auction.Services;

public class EmailService : IEmailService
{
    private readonly IEmailTemplateService _templateService;
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly SmtpMailOptions _smtpMailOptions;

    public EmailService(
        IEmailTemplateService templateService,
        IStringLocalizer<SharedResource> localizer,
        IOptions<SmtpMailOptions> options)
    {
        _templateService = templateService;
        _localizer = localizer;
        _smtpMailOptions = options.Value;
    }

    public async Task SendVerificationEmailAsync(string toEmail, string firstName, string verificationLink)
    {
        string mailBody = await _templateService.GenerateEmailVerificationEmailAsync(firstName, verificationLink);
        string subject = _localizer[EmailMessages.VerifyEmailSubject].Value;

        await SendEmailAsync(toEmail, subject, mailBody);
    }

    public async Task SendInvitationEmailAsync(string toEmail, string firstName, string invitationLink)
    {
        string mailBody = await _templateService.GenerateInvitationEmailAsync(firstName, invitationLink);
        string subject = _localizer[EmailMessages.InviteSubject].Value;

        await SendEmailAsync(toEmail, subject, mailBody);
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string firstName, string resetLink)
    {
        string mailBody = await _templateService.GeneratePasswordResetEmailAsync(firstName, resetLink);
        string subject = _localizer[EmailMessages.ResetPasswordSubject].Value;
        await SendEmailAsync(toEmail, subject, mailBody);
    }

    public async Task SendPasswordChangedEmailAsync(string toEmail, string firstName)
    {
        string mailBody = await _templateService.GeneratePasswordChangedEmailAsync(firstName);
        string subject = _localizer[EmailMessages.PasswordChangedSubject].Value;
        await SendEmailAsync(toEmail, subject, mailBody);
    }
    
    public async Task SendFacilityAssignedEmailAsync(string toEmail, string firstName, string facilityName, string dashboardLink)
    {
        string mailBody = await _templateService.GenerateFacilityAssignedEmailAsync(firstName, facilityName, dashboardLink);
        string subject = _localizer[EmailMessages.FacilityAssignedSubject].Value;
        await SendEmailAsync(toEmail, subject, mailBody);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        MimeMessage mailMessage = new();
        mailMessage.From.Add(new MailboxAddress(_smtpMailOptions.FromName, _smtpMailOptions.FromEmail));
        mailMessage.To.Add(MailboxAddress.Parse(toEmail));
        mailMessage.Subject = subject;
        mailMessage.Body = new TextPart(TextFormat.Html) { Text = htmlMessage };

        using SmtpClient smtpClient = new();
        smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
        await smtpClient.ConnectAsync(_smtpMailOptions.Host, _smtpMailOptions.Port, SecureSocketOptions.Auto);
        await smtpClient.AuthenticateAsync(_smtpMailOptions.Username, _smtpMailOptions.Password);

        await smtpClient.SendAsync(mailMessage);
        await smtpClient.DisconnectAsync(true);
    }
}