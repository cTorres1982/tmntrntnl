using JobTracker.Modules.Billing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTracker.Modules.Billing.Infrastructure.Persistence.Configurations;

internal sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("invoices");
        builder.HasKey(invoice => invoice.Id);

        builder.Property(invoice => invoice.IdempotencyKey).IsRequired().HasMaxLength(200);

        // Safety net against concurrent duplicate dispatch: even if two workers
        // process a redelivered JobCompletedIntegrationEvent at the same instant
        // (the application-level existence check in
        // GenerateInvoiceOnJobCompletedHandler has a race window), the database
        // itself refuses a second Invoice with the same idempotency key.
        builder.HasIndex(invoice => invoice.IdempotencyKey).IsUnique();

        builder.HasIndex(invoice => invoice.OrganizationId);

        builder.Ignore(invoice => invoice.DomainEvents);
    }
}
