using JobTracker.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTracker.Modules.Jobs.Infrastructure.Persistence.Configurations;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");
        builder.HasKey(message => message.Id);
        builder.Property(message => message.Type).IsRequired().HasMaxLength(500);
        builder.Property(message => message.Content).IsRequired().HasColumnType("jsonb");

        // The dispatcher's poll query is always "WHERE processed_on_utc IS NULL" —
        // a partial index only covers the rows that query needs, so it stays small
        // even as processed messages accumulate over time.
        builder.HasIndex(message => message.OccurredOnUtc)
            .HasDatabaseName("ix_outbox_messages_unprocessed")
            .HasFilter("processed_on_utc IS NULL");
    }
}
