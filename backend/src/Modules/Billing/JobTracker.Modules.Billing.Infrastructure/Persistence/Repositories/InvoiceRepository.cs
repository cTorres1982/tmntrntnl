using JobTracker.Modules.Billing.Domain;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Modules.Billing.Infrastructure.Persistence.Repositories;

internal sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly BillingDbContext _dbContext;

    public InvoiceRepository(BillingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsWithIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default) =>
        await _dbContext.Invoices.AnyAsync(invoice => invoice.IdempotencyKey == idempotencyKey, cancellationToken);

    public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default) =>
        await _dbContext.Invoices.AddAsync(invoice, cancellationToken);
}
