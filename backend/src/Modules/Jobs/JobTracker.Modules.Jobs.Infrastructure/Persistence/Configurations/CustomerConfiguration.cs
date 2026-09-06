using JobTracker.Modules.Jobs.Infrastructure.Persistence.Placeholders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTracker.Modules.Jobs.Infrastructure.Persistence.Configurations;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");
        builder.HasKey(customer => customer.Id);
        builder.Property(customer => customer.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(customer => customer.OrganizationId);
    }
}
