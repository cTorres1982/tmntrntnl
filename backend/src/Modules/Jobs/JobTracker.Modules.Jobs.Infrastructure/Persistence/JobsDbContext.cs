using JobTracker.Modules.Jobs.Application.Abstractions;
using JobTracker.Modules.Jobs.Domain;
using JobTracker.SharedKernel.Application;
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
    public JobsDbContext(DbContextOptions<JobsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Job> Jobs => Set<Job>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    IQueryable<Job> IJobsDbContext.Jobs => Jobs;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("jobs");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(JobsDbContext).Assembly);
    }
}
