using e_Sat_Auction.Common.Interfaces.Messaging;

namespace e_Sat_Auction.Features.User.InviteUser;

public record InviteUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string TargetRole) : IAuditableCommand<Guid>;