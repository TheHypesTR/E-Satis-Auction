using E_Satis_Auction.Common.Interfaces.Messaging;

namespace E_Satis_Auction.Features.User.InviteUser;

public record InviteUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string TargetRole) : IAuditableCommand<Guid>;