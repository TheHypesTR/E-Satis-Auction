using MediatR;

namespace e_Sat_Auction.Common.Interfaces;

public interface IHasDomainEvents
{
    IReadOnlyCollection<INotification> DomainEvents { get; }
    void ClearDomainEvents();
}