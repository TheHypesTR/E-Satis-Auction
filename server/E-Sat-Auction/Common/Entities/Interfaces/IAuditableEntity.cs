namespace e_Sat_Auction.Common.Entities.Interfaces;

public interface IAuditableEntity
{
    DateTime CreatedAt { get; }
    DateTime UpdatedAt { get; }
}