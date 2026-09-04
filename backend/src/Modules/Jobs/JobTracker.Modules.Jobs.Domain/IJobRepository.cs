namespace JobTracker.Modules.Jobs.Domain;

/// <summary>
/// Write-side contract for the <see cref="Job"/> aggregate. The read-optimized,
/// no-tracking/projected search used by SearchJobsQuery goes through a separate
/// read abstraction (Jobs.Application's IJobsDbContext) rather than through this
/// repository, since returning full tracked aggregates for a paginated list view
/// would defeat the point of a read-optimized query.
/// </summary>
public interface IJobRepository
{
    Task<Job?> GetByIdAsync(Guid id, Guid organizationId, CancellationToken cancellationToken = default);

    Task AddAsync(Job job, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Job>> SearchAsync(Guid organizationId, CancellationToken cancellationToken = default);
}
