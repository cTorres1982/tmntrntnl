using System.Text.Json;
using JobTracker.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace JobTracker.SharedKernel.Outbox;

/// <summary>
/// Runs before every SaveChanges: converts any domain events raised on tracked
/// aggregates into OutboxMessage rows inserted into the same DbContext/transaction,
/// then clears the events off the aggregates. Shared by every module's DbContext —
/// it only needs OutboxMessage to be part of that DbContext's model.
/// </summary>
public sealed class InsertOutboxMessagesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        InsertOutboxMessages(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        InsertOutboxMessages(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void InsertOutboxMessages(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        List<AggregateRoot> aggregatesWithEvents = context.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(entry => entry.Entity)
            .Where(aggregate => aggregate.DomainEvents.Count > 0)
            .ToList();

        List<OutboxMessage> outboxMessages = aggregatesWithEvents
            .SelectMany(aggregate => aggregate.DomainEvents)
            .Select(domainEvent => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = domainEvent.GetType().AssemblyQualifiedName!,
                Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                OccurredOnUtc = domainEvent.OccurredOnUtc,
            })
            .ToList();

        context.AddRange(outboxMessages);

        foreach (AggregateRoot aggregate in aggregatesWithEvents)
        {
            aggregate.ClearDomainEvents();
        }
    }
}
