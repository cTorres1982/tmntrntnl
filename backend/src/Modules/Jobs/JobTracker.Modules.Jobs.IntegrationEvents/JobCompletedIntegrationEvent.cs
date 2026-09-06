using MediatR;

namespace JobTracker.Modules.Jobs.IntegrationEvents;

/// <summary>
/// Public contract for the Jobs module's "a job was completed" fact — this is the
/// Open Host Service other modules integrate against. Billing references only this
/// project, never Jobs.Domain/Application/Infrastructure, so Jobs can change its
/// internal domain model freely without breaking Billing.
///
/// Published (via MediatR's in-process pub/sub) by
/// JobTracker.Modules.Jobs.Infrastructure.Outbox.ProcessOutboxMessagesJob when it
/// dispatches a persisted JobCompletedDomainEvent outbox row — see that class for
/// why the mapping happens at dispatch time rather than at outbox-write time.
/// </summary>
public sealed record JobCompletedIntegrationEvent(
    Guid JobId,
    Guid OrganizationId,
    Guid CustomerId,
    Guid? AssigneeId,
    DateTime CompletedAtUtc) : INotification;
