namespace JobTracker.SharedKernel.Application;

/// <summary>
/// Implemented by each module's own EF Core DbContext. SaveChangesAsync persists
/// changes and, via InsertOutboxMessagesInterceptor, converts any domain events
/// raised on tracked aggregates into outbox messages in the same transaction.
/// Shared across modules because its shape is purely technical — no module-specific
/// type appears here.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
