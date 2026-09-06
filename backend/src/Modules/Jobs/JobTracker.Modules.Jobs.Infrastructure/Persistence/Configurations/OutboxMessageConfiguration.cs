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
        builder.HasIndex(message => message.ProcessedOnUtc);
    }
}
