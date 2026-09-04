namespace JobTracker.Modules.Jobs.Application.Abstractions;

/// <summary>
/// Implemented by the Jobs EF Core DbContext (Infrastructure layer). SaveChangesAsync
/// persists changes and, via InsertOutboxMessagesInterceptor, converts any domain
/// events raised on tracked aggregates into outbox messages in the same transaction.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
