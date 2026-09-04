using JobTracker.SharedKernel.Domain;

namespace JobTracker.Modules.Jobs.Domain.Events;

public sealed record JobCancelledDomainEvent(
    Guid JobId,
    Guid OrganizationId,
    string Reason,
    DateTime OccurredOnUtc) : IDomainEvent;
