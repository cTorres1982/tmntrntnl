using JobTracker.Modules.Jobs.Domain;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Modules.Jobs.Infrastructure.Persistence.Repositories;

internal sealed partial class JobRepository : IJobRepository
{
    private readonly JobsDbContext _dbContext;

    public JobRepository(JobsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Job?> GetByIdAsync(Guid id, Guid organizationId, CancellationToken cancellationToken = default) =>
        await _dbContext.Jobs
            .FirstOrDefaultAsync(job => job.Id == id && job.OrganizationId == organizationId, cancellationToken);

    public async Task AddAsync(Job job, CancellationToken cancellationToken = default) =>
        await _dbContext.Jobs.AddAsync(job, cancellationToken);
}
