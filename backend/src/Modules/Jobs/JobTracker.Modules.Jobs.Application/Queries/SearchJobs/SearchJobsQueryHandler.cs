using JobTracker.Modules.Jobs.Application.Abstractions;
using JobTracker.Modules.Jobs.Domain;
using JobTracker.SharedKernel.Pagination;
using JobTracker.SharedKernel.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Modules.Jobs.Application.Queries.SearchJobs;

/// <summary>
/// Read-optimized: queries IJobsDbContext directly (no-tracking, projected to
/// JobResponse) instead of going through IJobRepository, which would materialize
/// full tracked aggregates — wasteful for a paginated list view.
/// </summary>
internal sealed class SearchJobsQueryHandler : IRequestHandler<SearchJobsQuery, Result<PagedList<JobResponse>>>
{
    private readonly IJobsDbContext _dbContext;

    public SearchJobsQueryHandler(IJobsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedList<JobResponse>>> Handle(SearchJobsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Job> query = _dbContext.Jobs
            .AsNoTracking()
            .Where(job => job.OrganizationId == request.OrganizationId);

        if (request.Statuses is { Count: > 0 })
        {
            query = query.Where(job => request.Statuses.Contains(job.Status));
        }

        if (request.FromDate is not null)
        {
            query = query.Where(job => job.ScheduledDate >= request.FromDate);
        }

        if (request.ToDate is not null)
        {
            query = query.Where(job => job.ScheduledDate <= request.ToDate);
        }

        if (request.AssigneeId is not null)
        {
            query = query.Where(job => job.AssigneeId == request.AssigneeId);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(job =>
                job.Title.Contains(request.SearchTerm) || job.Description.Contains(request.SearchTerm));
        }

        int totalCount = await query.CountAsync(cancellationToken);

        List<JobResponse> items = await query
            .OrderByDescending(job => job.CreatedAtUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(job => new JobResponse(
                job.Id,
                job.Title,
                job.Description,
                job.Status,
                job.ScheduledDate,
                job.AssigneeId,
                job.CustomerId,
                job.Address.Street,
                job.Address.City,
                job.Address.State,
                job.Address.ZipCode,
                job.Photos.Count,
                job.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return Result.Success(PagedList<JobResponse>.Create(items, request.Page, request.PageSize, totalCount));
    }
}
