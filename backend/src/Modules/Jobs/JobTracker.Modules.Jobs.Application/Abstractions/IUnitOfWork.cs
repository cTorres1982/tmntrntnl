namespace JobTracker.Modules.Jobs.Application.Abstractions;

/// <summary>
/// Implemented by JobsDbContext. Deliberately module-local, not shared from
/// SharedKernel: a shared IUnitOfWork type with one implementation per module
/// causes the modules' DI registrations to collide (DI resolves whichever
/// module's AddXInfrastructure() ran last for that single interface type) — each
/// module needs its own distinct interface even though the shape is identical.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
