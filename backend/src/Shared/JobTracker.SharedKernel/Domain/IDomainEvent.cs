using MediatR;

namespace JobTracker.SharedKernel.Domain;

public interface IDomainEvent : INotification
{
    /// <summary>
    /// Every domain event is tenant-scoped — this lets ProcessOutboxMessagesJob set
    /// the ambient ICurrentOrganizationProvider correctly before dispatching, since
    /// background job processing has no HTTP request/header to derive it from.
    /// </summary>
    Guid OrganizationId { get; }

    DateTime OccurredOnUtc { get; }
}
