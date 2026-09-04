using JobTracker.SharedKernel.Domain;

namespace JobTracker.Modules.Jobs.Domain.Events;

/// <summary>
/// Raised when a job is completed. Triggers invoice generation (Billing module) and a
/// customer notification via the outbox + <see cref="JobTracker.Modules.Jobs.IntegrationEvents"/>.
/// </summary>
public sealed record JobCompletedDomainEvent(
    Guid JobId,
    Guid OrganizationId,
    Guid CustomerId,
    Guid? AssigneeId,
    DateTime OccurredOnUtc) : IDomainEvent;
