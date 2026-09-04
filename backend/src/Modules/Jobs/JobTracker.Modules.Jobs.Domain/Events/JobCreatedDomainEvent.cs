using JobTracker.SharedKernel.Domain;

namespace JobTracker.Modules.Jobs.Domain.Events;

/// <summary>
/// Raised when a job is created. Consumed within the Jobs module to notify the assigned crew.
/// </summary>
public sealed record JobCreatedDomainEvent(
    Guid JobId,
    Guid OrganizationId,
    Guid? AssigneeId,
    DateTime OccurredOnUtc) : IDomainEvent;
