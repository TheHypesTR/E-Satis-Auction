using E_Satis_Auction.Common.Interfaces.Messaging;

namespace E_Satis_Auction.Features.Auth.ResetPassword;

public record ResetPasswordCommand(
    string EncryptedPayload,
    string NewPassword,
    string ConfirmPassword) : ICommand;