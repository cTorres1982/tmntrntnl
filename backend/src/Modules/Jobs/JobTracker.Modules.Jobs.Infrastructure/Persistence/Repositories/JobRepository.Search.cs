using JobTracker.Modules.Jobs.Domain;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Modules.Jobs.Infrastructure.Persistence.Repositories;

internal sealed partial class JobRepository
{
    public async Task<IReadOnlyList<Job>> SearchAsync(Guid organizationId, CancellationToken cancellationToken = default) =>
        await _dbContext.Jobs
            .Where(job => job.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);
}
