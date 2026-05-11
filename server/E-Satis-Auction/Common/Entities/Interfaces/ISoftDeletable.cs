namespace E_Satis_Auction.Common.Entities.Interfaces;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }
    void Delete();
}