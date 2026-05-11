using MediatR;

namespace E_Satis_Auction.Common.Interfaces;

public interface IHasDomainEvents
{
    IReadOnlyCollection<INotification> DomainEvents { get; }
    void ClearDomainEvents();
}