using System.ComponentModel.DataAnnotations.Schema;
using E_Satis_Auction.Common.Entities.Interfaces;
using E_Satis_Auction.Common.Interfaces;
using MediatR;

namespace E_Satis_Auction.Common.Entities;

public abstract class BaseEntity : IAuditableEntity, ISoftDeletable, IHasDomainEvents
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    
    [NotMapped]
    private readonly List<INotification> _domainEvents = [];

    [NotMapped]
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents;

    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    protected void AddDomainEvent(INotification domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}