using JobTracker.Modules.Jobs.Domain;

namespace JobTracker.Modules.Jobs.Application.Abstractions;

/// <summary>
/// Read-only seam onto the Jobs EF Core DbContext, used exclusively by query handlers
/// that need no-tracking, projected reads (e.g. SearchJobsQuery) — kept separate from
/// IJobRepository, which is the write-side contract for the Job aggregate.
/// </summary>
public interface IJobsDbContext
{
    IQueryable<Job> Jobs { get; }
}
