using e_Sat_Auction.Common.Interfaces.Messaging;

namespace e_Sat_Auction.Features.Auth.ResetPassword;

public record ResetPasswordCommand(
    string EncryptedPayload,
    string NewPassword,
    string ConfirmPassword) : ICommand;