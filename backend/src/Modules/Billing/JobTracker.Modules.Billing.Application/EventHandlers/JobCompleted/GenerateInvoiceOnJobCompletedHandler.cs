using JobTracker.Modules.Billing.Application.Abstractions;
using JobTracker.Modules.Billing.Domain;
using JobTracker.Modules.Jobs.IntegrationEvents;
using MediatR;

namespace JobTracker.Modules.Billing.Application.EventHandlers.JobCompleted;

/// <summary>
/// The other subscriber to JobCompletedIntegrationEvent (alongside Jobs' own
/// notification handler) — this is the cross-module reaction: Billing never
/// references Jobs.Domain/Application/Infrastructure, only the public
/// JobTracker.Modules.Jobs.IntegrationEvents contract.
///
/// Idempotent by construction: the outbox gives at-least-once delivery, so this
/// event can arrive more than once for the same job completion. Checking for an
/// existing Invoice with the same idempotency key before creating a new one (plus
/// the unique index in InvoiceConfiguration as a safety net against concurrent
/// duplicate dispatch) means a redelivery is a no-op rather than a duplicate
/// invoice.
/// </summary>
internal sealed class GenerateInvoiceOnJobCompletedHandler : INotificationHandler<JobCompletedIntegrationEvent>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public GenerateInvoiceOnJobCompletedHandler(
        IInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task Handle(JobCompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        string idempotencyKey = Invoice.BuildIdempotencyKey(notification.JobId, notification.CompletedAtUtc);

        if (await _invoiceRepository.ExistsWithIdempotencyKeyAsync(idempotencyKey, cancellationToken))
        {
            return;
        }

        Invoice invoice = Invoice.Create(
            notification.JobId,
            notification.OrganizationId,
            notification.CustomerId,
            notification.CompletedAtUtc,
            _timeProvider.GetUtcNow().UtcDateTime);

        await _invoiceRepository.AddAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
