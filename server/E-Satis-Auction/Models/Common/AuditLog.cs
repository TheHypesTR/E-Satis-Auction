namespace E_Satis_Auction.Models.Common;

public sealed class AuditLog
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; }
    public string UserEmail { get; private set; }
    public string IpAddress { get; private set; }
    public string Action { get; private set; }
    public string Details { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private AuditLog()
    {
        UserId = string.Empty;
        UserEmail = string.Empty;
        IpAddress = string.Empty;
        Action = string.Empty;
        Details = string.Empty;
    }

    public static AuditLog Create(string userId, string userEmail, string ipAddress, string action, string details)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UserEmail = userEmail,
            IpAddress = ipAddress,
            Action = action,
            Details = details,
            CreatedAt = DateTime.UtcNow
        };
    }
}