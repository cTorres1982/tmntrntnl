namespace JobTracker.Modules.Billing.Domain;

public interface IInvoiceRepository
{
    Task<bool> ExistsWithIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default);

    Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
}
