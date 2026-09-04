using JobTracker.Modules.Jobs.Domain;
using JobTracker.SharedKernel.Pagination;
using JobTracker.SharedKernel.Results;
using MediatR;

namespace JobTracker.Modules.Jobs.Application.Queries.SearchJobs;

public sealed record SearchJobsQuery(
    Guid OrganizationId,
    string? SearchTerm,
    IReadOnlyCollection<JobStatus>? Statuses,
    DateTime? FromDate,
    DateTime? ToDate,
    Guid? AssigneeId,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PagedList<JobResponse>>>;
