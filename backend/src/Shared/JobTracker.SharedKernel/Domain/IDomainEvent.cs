using MediatR;

namespace JobTracker.SharedKernel.Domain;

public interface IDomainEvent : INotification
{
    DateTime OccurredOnUtc { get; }
}
