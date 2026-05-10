using e_Sat_Auction.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace e_Sat_Auction.Common.Interceptors;

public sealed class DispatchDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IMediator _mediator;

    public DispatchDomainEventsInterceptor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        DbContext? dbContext = eventData.Context;
        if (dbContext is not null)
        {
            await DispatchDomainEventsAsync(dbContext, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchDomainEventsAsync(DbContext dbContext, CancellationToken cancellationToken)
    {
        while (true)
        {
            List<IHasDomainEvents> entitiesWithEvents = dbContext.ChangeTracker
                .Entries<IHasDomainEvents>()
                .Select(entry => entry.Entity)
                .Where(entity => entity.DomainEvents.Count > 0)
                .ToList();

            if (entitiesWithEvents.Count is 0)
            {
                break;
            }

            List<INotification> domainEvents = entitiesWithEvents
                .SelectMany(entity => entity.DomainEvents)
                .ToList();

            foreach (IHasDomainEvents entity in entitiesWithEvents)
            {
                entity.ClearDomainEvents();
            }

            foreach (INotification domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }
    }
}