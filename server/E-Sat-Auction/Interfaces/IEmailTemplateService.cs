namespace e_Sat_Auction.Interfaces;

public interface IEmailTemplateService
{
    Task<string> GenerateEmailVerificationEmailAsync(string firstName, string verificationLink);
    Task<string> GenerateInvitationEmailAsync(string firstName, string invitationLink);
    Task<string> GeneratePasswordResetEmailAsync(string firstName, string resetLink);
    Task<string> GeneratePasswordChangedEmailAsync(string firstName);
    Task<string> GenerateOrganizationAssignedEmailAsync(string firstName, string organizationName, string loginLink);
    Task<string> GenerateFacilityAssignedEmailAsync(string firstName, string organizationName, string loginLink);
}