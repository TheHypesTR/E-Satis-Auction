namespace E_Satis_Auction.Interfaces;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string toEmail, string firstName, string verificationLink);
    Task SendInvitationEmailAsync(string toEmail, string firstName, string invitationLink);
    Task SendPasswordResetEmailAsync(string toEmail, string firstName, string resetLink);
    Task SendPasswordChangedEmailAsync(string toEmail, string firstName);
    Task SendFacilityAssignedEmailAsync(string toEmail, string firstName, string organizationName, string dashboardLink);
}