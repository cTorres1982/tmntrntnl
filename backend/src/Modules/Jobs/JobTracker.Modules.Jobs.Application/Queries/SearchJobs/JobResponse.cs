using JobTracker.Modules.Jobs.Domain;

namespace JobTracker.Modules.Jobs.Application.Queries.SearchJobs;

public sealed record JobResponse(
    Guid Id,
    string Title,
    string Description,
    JobStatus Status,
    DateTime? ScheduledDate,
    Guid? AssigneeId,
    Guid CustomerId,
    string Street,
    string City,
    string State,
    string ZipCode,
    int PhotoCount,
    DateTime CreatedAtUtc);
