using System.Text.Json;
using JobTracker.Modules.Jobs.Domain.Events;
using JobTracker.Modules.Jobs.Infrastructure.Persistence;
using JobTracker.Modules.Jobs.IntegrationEvents;
using JobTracker.SharedKernel.Domain;
using JobTracker.SharedKernel.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JobTracker.Modules.Jobs.Infrastructure.Outbox;

/// <summary>
/// Registered as a Hangfire recurring job (wiring lives in JobTracker.Api's
/// composition root, Milestone M6) that polls jobs.outbox_messages and dispatches.
///
/// Every domain event is republished in-process via MediatR — this doubles as the
/// answer to "domain events WITHIN the module vs integration events ACROSS
/// modules": most domain events (e.g. JobCreatedDomainEvent) have no handler
/// registered anywhere, so republishing them is a harmless no-op — the "within
/// module" reaction, if any, would be a plain INotificationHandler&lt;TDomainEvent&gt;
/// living in Jobs.Application. JobCompletedDomainEvent is the one exception: it is
/// explicitly translated into JobCompletedIntegrationEvent, the public contract
/// Billing and the notification handler subscribe to — that translation, not the
/// dispatch mechanism, is what makes it "cross-module". The mechanism (outbox +
/// at-least-once delivery) is identical either way.
/// </summary>
public sealed class ProcessOutboxMessagesJob
{
    private const int BatchSize = 20;

    private readonly JobsDbContext _dbContext;
    private readonly IPublisher _publisher;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;

    public ProcessOutboxMessagesJob(JobsDbContext dbContext, IPublisher publisher, ILogger<ProcessOutboxMessagesJob> logger)
    {
        _dbContext = dbContext;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        List<OutboxMessage> messages = await _dbContext.OutboxMessages
            .Where(message => message.ProcessedOnUtc == null)
            .OrderBy(message => message.OccurredOnUtc)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        foreach (OutboxMessage message in messages)
        {
            await ProcessMessageAsync(message, cancellationToken);
        }
    }

    private async Task ProcessMessageAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        try
        {
            Type? eventType = Type.GetType(message.Type);
            if (eventType is null)
            {
                message.Error = $"Could not resolve CLR type '{message.Type}'.";
                message.ProcessedOnUtc = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(cancellationToken);
                return;
            }

            var domainEvent = (IDomainEvent?)JsonSerializer.Deserialize(message.Content, eventType);
            if (domainEvent is not null)
            {
                await DispatchAsync(domainEvent, cancellationToken);
            }

            message.ProcessedOnUtc = DateTime.UtcNow;
        }
        catch (Exception exception)
        {
            // Left unprocessed (ProcessedOnUtc stays null) so the next poll retries —
            // this is what gives the outbox at-least-once delivery instead of
            // silently dropping a failed dispatch.
            message.Error = exception.Message;
            _logger.LogError(exception, "Failed to process outbox message {MessageId}", message.Id);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        if (domainEvent is JobCompletedDomainEvent jobCompleted)
        {
            var integrationEvent = new JobCompletedIntegrationEvent(
                jobCompleted.JobId,
                jobCompleted.OrganizationId,
                jobCompleted.CustomerId,
                jobCompleted.AssigneeId,
                jobCompleted.OccurredOnUtc);

            await _publisher.Publish(integrationEvent, cancellationToken);
            return;
        }

        // Any other domain event (e.g. JobCreatedDomainEvent, JobCancelledDomainEvent)
        // is republished as-is. No handler is registered for these today, so this is
        // a no-op — the seam is here for a future within-module reaction.
        //
        // Must go through the `object` overload: domainEvent's static type here is
        // IDomainEvent, and Publish<TNotification>(domainEvent, ...) would bind
        // TNotification to IDomainEvent itself (it satisfies the INotification
        // constraint), making MediatR look for INotificationHandler<IDomainEvent>
        // instead of the concrete runtime type. The object overload dispatches by
        // the instance's actual runtime type instead.
        await _publisher.Publish((object)domainEvent, cancellationToken);
    }
}
