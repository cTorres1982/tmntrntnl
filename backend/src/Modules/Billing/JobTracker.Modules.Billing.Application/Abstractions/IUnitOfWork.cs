namespace JobTracker.Modules.Billing.Application.Abstractions;

/// <summary>
/// Implemented by BillingDbContext. Deliberately module-local, not shared from
/// SharedKernel — see JobTracker.Modules.Jobs.Application.Abstractions.IUnitOfWork
/// for why: a shared interface type with one implementation per module causes the
/// modules' DI registrations to collide.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
