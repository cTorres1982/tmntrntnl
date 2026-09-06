using JobTracker.Modules.Jobs.Infrastructure.Persistence.Placeholders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTracker.Modules.Jobs.Infrastructure.Persistence.Configurations;

internal sealed class AssigneeConfiguration : IEntityTypeConfiguration<Assignee>
{
    public void Configure(EntityTypeBuilder<Assignee> builder)
    {
        builder.ToTable("assignees");
        builder.HasKey(assignee => assignee.Id);
        builder.Property(assignee => assignee.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(assignee => assignee.OrganizationId);
    }
}
