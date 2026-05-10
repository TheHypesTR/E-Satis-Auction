namespace e_Sat_Auction.Common.Interfaces.Messaging;

public interface IPaginatedQuery
{
    int PageNumber { get; }
    int PageSize { get; }
}