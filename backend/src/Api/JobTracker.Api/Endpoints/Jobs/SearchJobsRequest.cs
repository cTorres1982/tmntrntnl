using JobTracker.Modules.Jobs.Domain;

namespace JobTracker.Api.Endpoints.Jobs;

public sealed record SearchJobsRequest(
    string? SearchTerm,
    JobStatus[]? Statuses,
    DateTime? FromDate,
    DateTime? ToDate,
    Guid? AssigneeId,
    int? Page,
    int? PageSize);
