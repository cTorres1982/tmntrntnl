using JobTracker.Modules.Jobs.Application.Abstractions;
using JobTracker.Modules.Jobs.IntegrationEvents;
using MediatR;

namespace JobTracker.Modules.Jobs.Application.EventHandlers.JobCompleted;

/// <summary>
/// One of two subscribers to JobCompletedIntegrationEvent (the other is Billing's
/// invoice handler) — both fire from the same MediatR Publish call in
/// ProcessOutboxMessagesJob. This one stays inside the Jobs module because
/// "notify the customer" is a direct consequence of Jobs' own event, not a concern
/// owned by another bounded context.
///
/// The customer's real email would come from the (out-of-scope) Contacts module —
/// see DataBase/ANALYSIS.md — so this uses a placeholder address derived from the
/// customer id.
/// </summary>
internal sealed class SendCustomerNotificationOnJobCompletedHandler : INotificationHandler<JobCompletedIntegrationEvent>
{
    private readonly INotificationService _notificationService;

    public SendCustomerNotificationOnJobCompletedHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public Task Handle(JobCompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        string placeholderCustomerEmail = $"customer-{notification.CustomerId:N}@example.com";

        return _notificationService.SendEmailAsync(
            placeholderCustomerEmail,
            "Your job has been completed",
            $"Job {notification.JobId} was completed on {notification.CompletedAtUtc:U}.",
            cancellationToken);
    }
}
