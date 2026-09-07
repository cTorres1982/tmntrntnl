using JobTracker.Modules.Jobs.Application.Abstractions;
using JobTracker.Modules.Jobs.Domain;
using JobTracker.SharedKernel.Multitenancy;
using JobTracker.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Modules.Jobs.Infrastructure.Persistence;

/// <summary>
/// SaveChangesAsync(CancellationToken) already matches IUnitOfWork's signature, so
/// DbContext satisfies that interface with no extra code — only the read-only
/// IJobsDbContext.Jobs projection needs an explicit implementation below.
/// </summary>
public sealed class JobsDbContext : DbContext, IJobsDbContext, IUnitOfWork
{
    private readonly ICurrentOrganizationProvider _currentOrganizationProvider;

    public JobsDbContext(DbContextOptions<JobsDbContext> options, ICurrentOrganizationProvider currentOrganizationProvider)
        : base(options)
    {
        _currentOrganizationProvider = currentOrganizationProvider;
    }

    public DbSet<Job> Jobs => Set<Job>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    IQueryable<Job> IJobsDbContext.Jobs => Jobs;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("jobs");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(JobsDbContext).Assembly);

        // Defense-in-depth: every command/query already takes OrganizationId
        // explicitly (see IJobRepository, SearchJobsQuery), but this global filter
        // means a handler that forgets to apply it still can't leak another
        // tenant's rows.
        modelBuilder.Entity<Job>().HasQueryFilter(job => job.OrganizationId == _currentOrganizationProvider.OrganizationId);
    }
}
